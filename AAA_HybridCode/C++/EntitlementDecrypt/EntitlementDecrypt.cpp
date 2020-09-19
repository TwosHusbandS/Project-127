// EntitlementDecrypt.cpp : Defines the exported functions for the DLL.
//

/*
 * This file is uses code from LegitimacyChecking from the CitizenFX Project - http://citizen.re/
 *
 * See the included licenses for licensing information on this code
 *
 * 
 */

#include "pch.h"
#include "framework.h"
#include <stdint.h>
#include "EntitlementDecrypt.h"
#include <vector>
#include <botan/block_cipher.h>
#include <botan/cbc.h>
#include <botan/filters.h>
#include <botan/pipe.h>
#include <EntitlementTables_1.h>
#include <EntitlementTables_2.h>

#define byte uint8_t

uint8_t* entitlement_tables_dec;
uint8_t* entitlement_key_dec;

void SetupEntitlementTables()
{
    if (!entitlement_tables_dec)
    {
        Botan::Pipe pipe(Botan::get_cipher("AES-256/CBC", Botan::SymmetricKey("7af5ce4e0224189eb1a0f7870a6a3c76bb10141dc26801ea37d142d96ff62675"), Botan::InitializationVector("58e742124051d7434ae7e0dbbfc89c23"), Botan::DECRYPTION));
        pipe.process_msg(entitlement_tables, entitlement_tables_len);

        auto decryptedTables = pipe.read_all(0);

        Botan::Pipe pipe2(Botan::get_cipher("AES-256/CBC", Botan::SymmetricKey("7af5ce4e0224189eb1a0f7870a6a3c76bb10141dc26801ea37d142d96ff62675"), Botan::InitializationVector("58e742124051d7434ae7e0dbbfc89c23"), Botan::DECRYPTION));
        pipe2.process_msg(entitlement_key, entitlement_key_len);

        auto decryptedKey = pipe2.read_all(0);

        entitlement_tables_dec = new uint8_t[decryptedTables.size()];
        memcpy(entitlement_tables_dec, decryptedTables.data(), decryptedTables.size());

        entitlement_key_dec = new uint8_t[decryptedKey.size()];
        memcpy(entitlement_key_dec, decryptedKey.data(), decryptedKey.size());
    }
}

class EntitlementBlockCipher : public Botan::BlockCipher
{
public:
    virtual Botan::BlockCipher* clone() const override
    {
        return new EntitlementBlockCipher();
    }

    virtual void clear() override
    {

    }

    virtual size_t block_size() const override
    {
        return 16;
    }

    virtual void encrypt_n(const byte in[], byte out[],
        size_t blocks) const override
    {
        throw std::exception();
    }

private:
    inline const uint32_t* GetSubkey(int idx) const
    {
        uint32_t* keyUints = (uint32_t*)entitlement_key_dec;
        return &keyUints[4 * idx];
    }

    inline const uint32_t* GetDecryptTable(int idx) const
    {
        uint32_t* dtUints = (uint32_t*)entitlement_tables_dec;
        return &dtUints[16 * 256 * idx];
    }

    inline const uint32_t* GetDecryptBytes(const uint32_t* tables, int idx) const
    {
        return &tables[256 * idx];
    }

public:
    virtual void decrypt_n(const byte in[], byte out[],
        size_t blocks) const override
    {
        SetupEntitlementTables();

        for (size_t i = 0; i < blocks; i++)
        {
            const byte* inBuf = &in[16 * i];
            byte* outBuf = &out[16 * i];

            DecryptRoundA(inBuf, outBuf, GetSubkey(0), GetDecryptTable(0));
            DecryptRoundA(outBuf, outBuf, GetSubkey(1), GetDecryptTable(1));

            for (int j = 2; j <= 15; j++)
            {
                DecryptRoundB(outBuf, outBuf, GetSubkey(j), GetDecryptTable(j));
            }

            DecryptRoundA(outBuf, outBuf, GetSubkey(16), GetDecryptTable(16));
        }
    }

private:
    void DecryptRoundA(const byte in[], byte out[], const uint32_t* key, const uint32_t* table) const
    {
        uint32_t x1 = GetDecryptBytes(table, 0)[in[0]] ^
            GetDecryptBytes(table, 1)[in[1]] ^
            GetDecryptBytes(table, 2)[in[2]] ^
            GetDecryptBytes(table, 3)[in[3]] ^
            key[0];

        uint32_t x2 = GetDecryptBytes(table, 4)[in[4]] ^
            GetDecryptBytes(table, 5)[in[5]] ^
            GetDecryptBytes(table, 6)[in[6]] ^
            GetDecryptBytes(table, 7)[in[7]] ^
            key[1];

        uint32_t x3 = GetDecryptBytes(table, 8)[in[8]] ^
            GetDecryptBytes(table, 9)[in[9]] ^
            GetDecryptBytes(table, 10)[in[10]] ^
            GetDecryptBytes(table, 11)[in[11]] ^
            key[2];

        uint32_t x4 = GetDecryptBytes(table, 12)[in[12]] ^
            GetDecryptBytes(table, 13)[in[13]] ^
            GetDecryptBytes(table, 14)[in[14]] ^
            GetDecryptBytes(table, 15)[in[15]] ^
            key[3];

        *(uint32_t*)&out[0] = x1;
        *(uint32_t*)&out[4] = x2;
        *(uint32_t*)&out[8] = x3;
        *(uint32_t*)&out[12] = x4;
    }

    void DecryptRoundB(const byte in[], byte out[], const uint32_t* key, const uint32_t* table) const
    {
        uint32_t x1 = GetDecryptBytes(table, 0)[in[0]] ^
            GetDecryptBytes(table, 7)[in[7]] ^
            GetDecryptBytes(table, 10)[in[10]] ^
            GetDecryptBytes(table, 13)[in[13]] ^
            key[0];

        uint32_t x2 = GetDecryptBytes(table, 1)[in[1]] ^
            GetDecryptBytes(table, 4)[in[4]] ^
            GetDecryptBytes(table, 11)[in[11]] ^
            GetDecryptBytes(table, 14)[in[14]] ^
            key[1];

        uint32_t x3 = GetDecryptBytes(table, 2)[in[2]] ^
            GetDecryptBytes(table, 5)[in[5]] ^
            GetDecryptBytes(table, 8)[in[8]] ^
            GetDecryptBytes(table, 15)[in[15]] ^
            key[2];

        uint32_t x4 = GetDecryptBytes(table, 3)[in[3]] ^
            GetDecryptBytes(table, 6)[in[6]] ^
            GetDecryptBytes(table, 9)[in[9]] ^
            GetDecryptBytes(table, 12)[in[12]] ^
            key[3];

        out[0] = (byte)((x1 >> 0) & 0xFF);
        out[1] = (byte)((x1 >> 8) & 0xFF);
        out[2] = (byte)((x1 >> 16) & 0xFF);
        out[3] = (byte)((x1 >> 24) & 0xFF);
        out[4] = (byte)((x2 >> 0) & 0xFF);
        out[5] = (byte)((x2 >> 8) & 0xFF);
        out[6] = (byte)((x2 >> 16) & 0xFF);
        out[7] = (byte)((x2 >> 24) & 0xFF);
        out[8] = (byte)((x3 >> 0) & 0xFF);
        out[9] = (byte)((x3 >> 8) & 0xFF);
        out[10] = (byte)((x3 >> 16) & 0xFF);
        out[11] = (byte)((x3 >> 24) & 0xFF);
        out[12] = (byte)((x4 >> 0) & 0xFF);
        out[13] = (byte)((x4 >> 8) & 0xFF);
        out[14] = (byte)((x4 >> 16) & 0xFF);
        out[15] = (byte)((x4 >> 24) & 0xFF);
    }

public:
    virtual Botan::Key_Length_Specification key_spec() const override
    {
        return Botan::Key_Length_Specification(272);
    }

    virtual void key_schedule(const byte key[], size_t length) override
    {
        // yeah you wish
    }

    virtual std::string name() const override
    {
        return "RAEA(tm) (c) OpenIV-Putin-Team";
    }
};

static uint32_t BigLong(uint32_t val)
{
    val = ((val << 8) & 0xFF00FF00) | ((val >> 8) & 0xFF00FF);
    return (val << 16) | (val >> 16);
}

static uint64_t BigLongLong(uint64_t val)
{
    val = ((val << 8) & 0xFF00FF00FF00FF00ULL) | ((val >> 8) & 0x00FF00FF00FF00FFULL);
    val = ((val << 16) & 0xFFFF0000FFFF0000ULL) | ((val >> 16) & 0x0000FFFF0000FFFFULL);
    return (val << 32) | (val >> 32);
}

/*class EntitlementBlock
{
public:
    static std::shared_ptr<EntitlementBlock> Read(const uint8_t* buffer, const uint8_t* rsaPubKey = nullptr);

public:
    inline uint64_t GetRockstarId() const
    {
        return m_rockstarId;
    }

    inline const std::string& GetMachineHash() const
    {
        return m_machineHash;
    }

    inline const std::string& GetXml() const
    {
        return m_xml;
    }

    inline time_t GetExpirationDate() const
    {
        return m_date;
    }

    inline bool IsValid() const
    {
        return m_valid;
    }

private:
    uint64_t m_rockstarId;
    std::string m_machineHash;
    std::string m_xml;
    time_t m_date;
    bool m_valid;
};
*/

// This is an example of an exported function.
extern "C" ENTITLEMENTDECRYPT_API bool fnEntitlementDecrypt(uint8_t * in, int len)
{
    Botan::secure_vector<uint8_t> blob(in, in + len);
    auto cbc = new Botan::CBC_Decryption(new EntitlementBlockCipher(), new Botan::Null_Padding());
    cbc->start(&blob[4], 16);

    cbc->finish(blob, 20);

    uint8_t* vecArr = &blob[0];

    for (int i = 0; i < len; i++) {
        in[i] = vecArr[i];
    }


    blob.clear();
    delete cbc;

    return true;
}