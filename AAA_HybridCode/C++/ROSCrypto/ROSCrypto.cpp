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
 //#include <base64.h>
#include <stdint.h>
#include <sstream>
#include "ROSCrypto.h"
#include <vector>
#include <botan/block_cipher.h>
#include <botan/cbc.h>
#include <botan/filters.h>
#include <botan/pipe.h>
#include <botan/base64.h>
#include <botan/botan.h>
#include <botan/stream_cipher.h>
#include <EntitlementTables_1.h>
#include <EntitlementTables_2.h>

#define ROS_PLATFORM_KEY_LAUNCHER "C6fU6TQTPgUVmy3KIB5g8ElA7DrenVpGogSZjsh+lqJaQHqv4Azoctd/v4trfX6cBjbEitUKBG/hRINF4AhQqcg="

#define byte uint8_t

uint8_t* entitlement_tables_dec;
uint8_t* entitlement_key_dec;

int qmin(int a, int b) {
    return a < b ? a : b;
}

static const std::string base64_chars =
"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
"abcdefghijklmnopqrstuvwxyz"
"0123456789+/";


static inline bool is_base64(uint8_t c) {
    return (isalnum(c) || (c == '+') || (c == '/'));
}

uint8_t* base64_decode(std::string const& encoded_string, size_t* outlen) {
    int in_len = encoded_string.size();
    int i = 0;
    int j = 0;
    int in_ = 0;
    uint8_t char_array_4[4], char_array_3[3];
    std::vector<uint8_t> ret;

    while (in_len-- && (encoded_string[in_] != '=') && is_base64(encoded_string[in_])) {
        char_array_4[i++] = encoded_string[in_]; in_++;
        if (i == 4) {
            for (i = 0; i < 4; i++)
                char_array_4[i] = base64_chars.find(char_array_4[i]);

            char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
            char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
            char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

            for (i = 0; (i < 3); i++)
                ret.push_back(char_array_3[i]);
            i = 0;
        }
    }

    if (i) {
        for (j = i; j < 4; j++)
            char_array_4[j] = 0;

        for (j = 0; j < 4; j++)
            char_array_4[j] = base64_chars.find(char_array_4[j]);

        char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
        char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
        char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

        for (j = 0; (j < i - 1); j++) ret.push_back(char_array_3[j]);
    }
    *outlen = ret.size();
    return &ret[0];
}

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


class ROSCryptoState
{
private:
    Botan::StreamCipher* m_rc4;

    uint8_t m_rc4Key[32];
    uint8_t m_xorKey[16];
    uint8_t m_hashKey[16];

public:
    ROSCryptoState()
    {
        // initialize the key inputs
        size_t outLength;
        uint8_t* platformStr;
        platformStr = base64_decode(ROS_PLATFORM_KEY_LAUNCHER, &outLength);

        memcpy(m_rc4Key, &platformStr[1], sizeof(m_rc4Key));
        memcpy(m_xorKey, &platformStr[33], sizeof(m_xorKey));
        memcpy(m_hashKey, &platformStr[49], sizeof(m_hashKey));

        //free(platformStr);

        // create the RC4 cipher and decode the keys
        m_rc4 = Botan::StreamCipher::create("RC4")->clone();//Botan::get_stream_cipher("RC4")->clone();

        // set the key
        m_rc4->set_key(m_rc4Key, sizeof(m_rc4Key));

        // decode the xor key
        m_rc4->cipher1(m_xorKey, sizeof(m_xorKey));

        // reset the key
        m_rc4->set_key(m_rc4Key, sizeof(m_rc4Key));

        // decode the hash key
        m_rc4->cipher1(m_hashKey, sizeof(m_hashKey));

        // and we're done
        delete m_rc4;
    }

    inline const uint8_t* GetXorKey()
    {
        return m_xorKey;
    }

    inline const uint8_t* GetHashKey()
    {
        return m_hashKey;
    }
};

extern "C" ROSCRYPTO_API void fnDecryptROSData(char* data, int size, const char* skcs, int* out_len)
{
    // initialize state
    ROSCryptoState state;

    std::string sessionKey(skcs);

    // read the packet RC4 key from the packet
    uint8_t rc4Key[16];

    bool hasSecurity = (!sessionKey.empty());

    uint8_t sessKey[16];

    if (hasSecurity)
    {
        auto keyData = Botan::base64_decode(sessionKey);
        memcpy(sessKey, keyData.data(), sizeof(sessKey));
    }

    for (int i = 0; i < sizeof(rc4Key); i++)
    {
        rc4Key[i] = data[i] ^ state.GetXorKey()[i];

        if (hasSecurity)
        {
            rc4Key[i] ^= sessKey[i];
        }
    }

    // initialize RC4 with the packet key
    Botan::StreamCipher* rc4 = Botan::StreamCipher::create("RC4")->clone();//Botan::get_stream_cipher("RC4")->clone();
    rc4->set_key(rc4Key, sizeof(rc4Key));

    // read the block size from the data
    uint8_t blockSizeData[4];
    //uint8_t blockSizeDataLE[4];
    rc4->cipher(reinterpret_cast<const uint8_t*>(&data[16]), blockSizeData, 4);

    // swap endianness
    //blockSizeDataLE[3] = blockSizeData[0];
    //blockSizeDataLE[2] = blockSizeData[1];
    //blockSizeDataLE[1] = blockSizeData[2];
    //blockSizeDataLE[0] = blockSizeData[3];

    uint32_t blockSize = (blockSizeData[0] << 24) + (blockSizeData[1] << 16) +
        (blockSizeData[2] << 8) + blockSizeData[1] + 20;

    // create a buffer for the block
    std::vector<uint8_t> blockData(blockSize);

    // a result stringstream as well
    std::stringstream result;

    // loop through packet blocks
    size_t start = 20;



    std::vector<uint8_t> encbuffer;
    encbuffer.reserve(size);

    for (int i = 0; i < 4; i++)
    {
        encbuffer.push_back(data[16 + i]);
    }

    while (start < size)
    {
        // calculate the end of this block
        int end = qmin(size, start + blockSize);

        // remove the size of the SHA1 hash from the end
        end -= 20;

        int thisLen = end - start;

        // decrypt the block
        //rc4->cipher(reinterpret_cast<const uint8_t*>(&data[start]), &blockData[0], thisLen);
        std::copy(&data[start], &data[start] + thisLen, std::back_inserter(encbuffer));

        // TODO: compare the resulting hash

        // append to the result buffer
        //result << std::string(reinterpret_cast<const char*>(&blockData[0]), thisLen);

        // increment the counter
        start += blockSize;
    }

    *out_len = encbuffer.size();

    std::copy(encbuffer.begin(), encbuffer.end(), data);


    rc4->clear();
    rc4->set_key(rc4Key, sizeof(rc4Key));
    rc4->cipher1((uint8_t*)data, *out_len);

    delete rc4;
    //delete& sessionKey;


    //const std::string& outs = result.str();
    //out = &std::vector<uint8_t>(outs.begin(), outs.end())[0];
    //*out_len = outs.length();
}

// This is an example of an exported function.
extern "C" ROSCRYPTO_API bool fnEntitlementDecrypt(uint8_t * in, int len)
{
    Botan::secure_vector<uint8_t> blob(in, in + len);
    auto cbc = new Botan::CBC_Decryption(new EntitlementBlockCipher(), new Botan::Null_Padding());
    cbc->start(&blob[4], 16);

    cbc->finish(blob, 20);

    //uint8_t* vecArr = &blob[0];

    std::copy(blob.begin(), blob.end(), in);
    /*for (int i = 0; i < len; i++) {
        in[i] = vecArr[i];
    }*/


    blob.clear();
    delete cbc;

    return true;
}


extern "C" ROSCRYPTO_API void fnEntitlementMap(uint8_t * buffer, uint32_t * length, uint8_t * *sig, uint32_t * sigSize)
{
    *length = BigLong(*(uint32_t*)buffer);
    buffer += 12;
    printf("\r\nBuffer Length: %d\r\n", *length);

    uint32_t machineHashSize = BigLong(*(uint32_t*)buffer);
    buffer += 4;
    //*machineHash = buffer;
    buffer += machineHashSize;

    uint32_t dateSize = BigLong(*(uint32_t*)buffer);
    buffer += 4;
    //*date = buffer;
    buffer += dateSize;

    uint32_t xmlSize = BigLong(*(uint32_t*)buffer);
    buffer += 4;
    //*xml = buffer;
    buffer += xmlSize;

    *sigSize = BigLong(*(uint32_t*)buffer);
    buffer += 4;
    *sig = buffer;

}

//extern "C" ROSCRYPTO_API void 