// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the ENTITLEMENTDECRYPT_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// ENTITLEMENTDECRYPT_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef ROSCRYPTO_EXPORTS
#define ROSCRYPTO_API __declspec(dllexport)
#else
#define ENTITLEMENTDECRYPT_API __declspec(dllimport)
#endif


//extern ENTITLEMENTDECRYPT_API int nEntitlementDecrypt;

extern "C" ROSCRYPTO_API bool fnEntitlementDecrypt(uint8_t* in, int len);
