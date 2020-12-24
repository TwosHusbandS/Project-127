// MTLInterface.cpp : Defines the exported functions for the DLL.
//

#include "pch.h"
#include "framework.h"
#include "MTLInterface.h"
#include <string>
#include <vector>
#include <algorithm>
#include <functional>
#include <Psapi.h>
#include <TlHelp32.h>

#pragma warning(disable : 4996)

class patchHandler {

private:
    ULONG_PTR base = NULL;
    HANDLE proc = NULL;
    bool internalHandle = false;

    void construct() {
        HMODULE modules;
        DWORD cbneeded;
        if (!EnumProcessModules(proc, &modules, sizeof(HMODULE), &cbneeded)) {
            //LOGF("Unable to retrieve modules!\r\n");
            proc = NULL;
        }
        base = (ULONG_PTR)modules;
    }

public:
    
    patchHandler(DWORD pid, std::wstring moduleName) {
        internalHandle = true;
        proc = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ
            | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, pid);
        if (proc == NULL) {
            //LOGF("Openprocess failed!\r\n");
        }
        chmodule(moduleName);
    }

    patchHandler(DWORD pid) {
        internalHandle = true;
        proc = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ
            | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, false, pid);
        if (proc == NULL) {
            //LOGF("Openprocess failed!\r\n");
        }
        construct();
    }

    patchHandler(HANDLE p, std::wstring moduleName) {
        proc = p;
        chmodule(moduleName);
    }

    patchHandler(HANDLE p) {
        proc = p;
        construct();
    }

    ~patchHandler() {
        if (internalHandle) {
            CloseHandle(proc);
        }
    }

    bool chmodule(std::wstring target) {
        if (proc == NULL) {
            return false;
        }
        HMODULE modules[1024];
        DWORD cbneeded;
        ULONG_PTR base_orig = base;
        base = NULL;
        if (EnumProcessModules(proc, modules, sizeof(modules), &cbneeded)) {
            //LOGF("Searching modules... (target: %ls)\r\n", target.c_str());
            for (int i = 0; i < (cbneeded / sizeof(HMODULE)); i++) {
                wchar_t moduleName[MAX_PATH];
                if (GetModuleFileNameExW(proc, modules[i], moduleName, MAX_PATH)) {
                    std::wstring modNoPath(moduleName);
                    size_t fileStartPos = modNoPath.find_last_of(L'\\');
                    modNoPath = modNoPath.substr(fileStartPos + 1);
                    //LOGF("Found module: %ls\r\n", modNoPath.c_str());
                    if (_wcsicmp(target.c_str(), modNoPath.c_str()) == 0) {
                        base = (ULONG_PTR)modules[i];
                    }
                }
            }
        }
        else {
            //LOGF("Unable to retrieve modules!\r\n");
            exit(-1);
        }
        if (base == NULL) {
            base = base_orig;
            return false;
        }
        return true;
    }

    bool write(uint64_t rva, uint8_t* bytes, size_t count, size_t* n_read = nullptr) {
        if (proc == NULL) {
            return false;
        }
        return WriteProcessMemory(proc, (void*)(base + rva), bytes, count, n_read);
    }

    bool read(uint64_t rva, uint8_t* bytes, size_t count, size_t* n_read = nullptr) {
        if (proc == NULL) {
            return false;
        }
        return ReadProcessMemory(proc, (void*)(base + rva), bytes, count, n_read);
    }

    HANDLE getHandle() {
        return proc;
    }
};

class patchHandlerEx : public patchHandler {
private:
    uint8_t headers[4096];
public:

    patchHandlerEx(DWORD pid) : patchHandler(pid) {
        read(0, headers, 4096);
    }

    patchHandlerEx(HANDLE p) : patchHandler(p) {
        read(0, headers, 4096);
    }

    patchHandlerEx(HANDLE p, std::wstring moduleName) : patchHandler(p, moduleName) {
        read(0, headers, 4096);
    }

    patchHandlerEx(DWORD p, std::wstring moduleName) : patchHandler(p, moduleName) {
        read(0, headers, 4096);
    }

    bool chmodule(std::wstring target) {
        bool out = patchHandler::chmodule(target);
        if (out) {
            read(0, headers, 4096);
        }
        return out;
    }

    void waitForModule(std::wstring module, int maxDelay = 30) {
        for (int i = 0; i < maxDelay * 10; i++) {
            if (!chmodule(module)) {
                Sleep(100);
            }
            else {
                break;
            }

        }
    }

    uint64_t findRVA(uint8_t* pattern, size_t patternSize) {
        if (getHandle() == NULL) {
            return 0;
        }
        IMAGE_DOS_HEADER* hdos = (IMAGE_DOS_HEADER*)&headers;
        IMAGE_NT_HEADERS* hnt = (IMAGE_NT_HEADERS*)&headers[hdos->e_lfanew];
        size_t fullsize = hnt->OptionalHeader.SizeOfCode;
        std::vector<uint8_t> codeFull(fullsize);
        read(0, &codeFull[0], fullsize);
        uint8_t* end = &codeFull[0] + fullsize;
        auto it = std::search(&codeFull[0], end, std::boyer_moore_horspool_searcher(pattern, pattern + patternSize));

        return (uint64_t)(it - &codeFull[0]);
    }
};

DWORD getPID(std::wstring procname) {
    PROCESSENTRY32W entry;
    entry.dwSize = sizeof(PROCESSENTRY32W);
    HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
    if (Process32FirstW(snapshot, &entry)) {
        while (Process32NextW(snapshot, &entry)) {
            if (!_wcsicmp(procname.c_str(), entry.szExeFile)) {
                return entry.th32ProcessID;
            }
        }
    }
    return 0;
}

static uint8_t pattern[] = { 0x84, 0xC0, 0x75, 0x19, 0xFF, 0xC7, 0x83, 0xFF,
    0x01, 0x7C, 0xD8 };

static uint8_t blob[16384];

extern "C" MTLINTERFACE_API bool retrieveMtlData(uint8_t* ticket, uint8_t* sessticket, 
    uint8_t* sessionKey, uint8_t* rockstarNick, uint8_t* countryCode,
    int32_t* posixtime, uint64_t* rockstarID) {

    DWORD pid = getPID(L"Launcher.exe");
    if (!pid) {
        return false;
    }
    patchHandlerEx sc(pid);

    if (!sc.chmodule(L"socialclub.dll")) {
        return false;
    }

    uint64_t target_rva = sc.findRVA(pattern, sizeof(pattern));
    if (target_rva == 0) {
        return false;
    }
    target_rva -= 35;
    uint32_t exe_indicated_offset;
    sc.read(target_rva, (uint8_t*)&exe_indicated_offset, sizeof(uint32_t));
    target_rva += 4 + (uint64_t)exe_indicated_offset;
    sc.read(target_rva, blob, sizeof(blob));
    memcpy((uint8_t*)posixtime, blob + 0x32C0, 4);
    if (posixtime == 0) {
        return false;
    }
    memcpy((uint8_t*)rockstarID, blob + 0xEE8, 8);
    memcpy((uint8_t*)sessionKey, blob + 0x10D8, 16);
    strcpy((char*)ticket, (char*)(blob + 0xAF0));
    strcpy((char*)sessticket, (char*)(blob + 0xCF0));
    strcpy((char*)rockstarNick, (char*)(blob + 0xE9F));
    strcpy((char*)countryCode, (char*)(blob + 0xE0C));
    return true;
}