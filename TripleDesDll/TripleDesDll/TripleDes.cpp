#include "TripleDes.h"
#include <iostream>
#include <string>
#include <vector>
#include <openssl/des.h>
#include <stdio.h>
#include "openssl/sha.h"  
#include "openssl/rsa.h"  
#include "openssl/rand.h"  
#include "openssl/objects.h"  
#include "openssl/pem.h"  
#include "openssl/md5.h" 
#include "openssl/x509.h"
#include "openssl/x509_vfy.h"
#include "openssl/x509v3.h"
 
#include "openssl/pkcs12.h"
#include "openssl/pkcs7.h"
#include "openssl/pkcs12.h"

using namespace std;

#pragma comment(lib,"libeay32.lib")
#pragma comment(lib,"ssleay32.lib")

extern "C"{
#include "openssl/applink.c"
};

// ---- rsa非对称加解密 ---- //    
#define KEY_LENGTH  2048               // 密钥长度  
#define PUB_KEY_FILE "pubkey.pem"    // 公钥路径  
#define PRI_KEY_FILE "prikey.pem"    // 私钥路径  


unsigned char cbc_iv[8] = { '0', '1', 'A', 'B', 'a', 'b', '9', '8' };
const string strIV = "01ABab98";
char* keyDefault = "ceshiceshiceshiceshiceshiceshiceshiceshi";
char* IVDefault = "01ABab98";
RSA* m_rsa = NULL;
EVP_PKEY *m_pKey = NULL;
//X509 *m_pCACert = NULL;

// 2018.09.13  begin
string Uchar2HEX(unsigned char ch)
{
	const static char hex[] = "0123456789ABCDEF";
	string strRet;
	strRet += hex[(ch >> 4) & 0xF];
	strRet += hex[ch & 0xF];
	return strRet;
}


std::string str2hex(const std::string src_str, bool upper)
{
	const static char hex1[] = "0123456789abcdef";
	const static char hex2[] = "0123456789ABCDEF";
	const char* hexp = upper ? hex2 : hex1;
	std::string dst;
	dst.assign(src_str.size() * 2, ' ');
	for (size_t i = 0; i < src_str.size(); ++i)
	{
		unsigned char c = (unsigned char)src_str[i];
		dst[i * 2 + 1] = hexp[c & 0xF];
		dst[i * 2] = hexp[(c >> 4) & 0xF];
	}
	return dst;
}




std::string Memory2hex(const char* src_mem, int nLenth, bool upper)
{
	const static char hex1[] = "0123456789abcdef";
	const static char hex2[] = "0123456789ABCDEF";
	const char* hexp = upper ? hex2 : hex1;
	std::string dst;
	dst.assign(nLenth * 2, ' ');
	for (size_t i = 0; i < nLenth; ++i)
	{
		unsigned char c = (unsigned char)src_mem[i];
		dst[i * 2 + 1] = hexp[c & 0xF];
		dst[i * 2] = hexp[(c >> 4) & 0xF];
	}
	return dst;
}

std::string hex2str(const std::string src_str)
{
	std::string dst;
	dst.assign(src_str.size() / 2, ' ');

	for (size_t i = 0; i < src_str.size(); i += 2)
	{
		char cHigh = (char)src_str[i];
		char cLow = (char)src_str[i + 1];


		if (cHigh >= '0' && cHigh <= '9')
		{
			cHigh = cHigh - '0';
		}
		else if (cHigh >= 'A' && cHigh <= 'F')
		{
			cHigh = cHigh - 'A' + 10;
		}
		else if (cHigh >= 'a' && cHigh <= 'f')
		{
			cHigh = cHigh - 'a' + 10;
		}

		if (cLow >= '0' && cLow <= '9')
		{
			cLow = cLow - '0';
		}
		else if (cLow >= 'A' && cLow <= 'F')
		{
			cLow = cLow - 'A' + 10;
		}
		else if (cLow >= 'a' && cLow <= 'f')
		{
			cLow = cLow - 'a' + 10;
		}


		char Ret = (cHigh << 4) + cLow;
		dst[i / 2] = Ret;
	}
	return dst;
}

// add by fdp 20181123
std::string hex2str(const char* src_str, int iSrcLen)
{
	std::string dst;
	dst.assign(iSrcLen / 2, ' ');

	for (size_t i = 0; i < iSrcLen; i += 2)
	{
		char cHigh = (char)src_str[i];
		char cLow = (char)src_str[i + 1];


		if (cHigh >= '0' && cHigh <= '9')
		{
			cHigh = cHigh - '0';
		}
		else if (cHigh >= 'A' && cHigh <= 'F')
		{
			cHigh = cHigh - 'A' + 10;
		}
		else if (cHigh >= 'a' && cHigh <= 'f')
		{
			cHigh = cHigh - 'a' + 10;
		}

		if (cLow >= '0' && cLow <= '9')
		{
			cLow = cLow - '0';
		}
		else if (cLow >= 'A' && cLow <= 'F')
		{
			cLow = cLow - 'A' + 10;
		}
		else if (cLow >= 'a' && cLow <= 'f')
		{
			cLow = cLow - 'a' + 10;
		}


		char Ret = (cHigh << 4) + cLow;
		dst[i / 2] = Ret;
	}
	return dst;
}

char* ConstChar2Char(const char* src)
{
	int nLen = strlen(src);
	char* pc = new char[nLen + 1];

	for (int i = 0; i < nLen; i++)
	{
		pc[i] = src[i];
	}
	pc[nLen] = '\0';

	return pc;
}

string DES_TRIPLE_CBC_Encrypt(const string desKey, const string data, const string iv)
{
	int keyLen = 0, x = 0;
	long dataLen = 0;
	DES_key_schedule ks1, ks2, ks3;
	unsigned char ke1[8], ke2[8], ke3[8], ivec[8];
	unsigned char input[8192], output[8192];

	keyLen = desKey.length();
	dataLen = data.length();

	if (keyLen == 0 || dataLen == 0)
		return "";

	memset(input, 0, 8192);
	memset(output, 0, 8192);
	memcpy(input, data.c_str(), dataLen);


	memset(ke1, 0, 8);
	memset(ke2, 0, 8);
	memset(ke3, 0, 8);

	if (keyLen == 16)
	{
		memcpy(ke1, desKey.c_str(), 8);
		memcpy(ke2, desKey.c_str() + 8, 8);
		memcpy(ke3, desKey.c_str(), 8);
	}
	else
	{
		memcpy(ke1, desKey.c_str(), 8);
		memcpy(ke2, desKey.c_str() + 8, 8);
		memcpy(ke3, desKey.c_str() + 16, 8);
	}

	DES_set_key_unchecked((const_DES_cblock*)ke1, &ks1);
	DES_set_key_unchecked((const_DES_cblock*)ke2, &ks2);
	DES_set_key_unchecked((const_DES_cblock*)ke3, &ks3);

	memcpy(ivec, iv.c_str(), 8);
	DES_ede3_cbc_encrypt(input, output, dataLen, &ks1, &ks2, &ks3, (DES_cblock *)ivec, DES_ENCRYPT);

	// 2018.09.17 begin
	int nBlockCount = dataLen % 8 ? dataLen / 8 + 1 : dataLen / 8;
	const static char hex[] = "0123456789ABCDEF";
	std::string dst;
	dst.assign(nBlockCount * 8 * 2, ' ');
	for (size_t i = 0; i < nBlockCount * 8; ++i)
	{
		unsigned char c = (unsigned char)output[i];
		dst[i * 2 + 1] = hex[c & 0xF];
		dst[i * 2] = hex[(c >> 4) & 0xF];
	}

	return dst;
	// 2018.09.17 end

// 	string encData;
// 	if (dataLen > 0)
// 	{
// 		string result((char *)output);
// 		encData = result;
// 	}
// 
// 	return encData;
}

string DES_TRIPLE_CBC_Decrypt(const string desKey, const string data, const string iv)
{
	int keyLen = 0, x = 0;
	long dataLen = 0;
	DES_key_schedule ks1, ks2, ks3;
	unsigned char ke1[8], ke2[8], ke3[8], ivec[8];
	unsigned char input[8192], output[8192];

	keyLen = desKey.length();
	dataLen = data.length();

	if (keyLen == 0 || dataLen == 0)
		return "";

	memset(input, 0, 8192);
	memset(output, 0, 8192);
	memcpy(input, data.c_str(), dataLen);


	memset(ke1, 0, 8);
	memset(ke2, 0, 8);
	memset(ke3, 0, 8);

	memcpy(ke1, desKey.c_str(), 8);
	memcpy(ke2, desKey.c_str() + 8, 8);
	memcpy(ke3, desKey.c_str() + 16, 8);

	DES_set_key_unchecked((const_DES_cblock*)ke1, &ks1);
	DES_set_key_unchecked((const_DES_cblock*)ke2, &ks2);
	DES_set_key_unchecked((const_DES_cblock*)ke3, &ks3);


	memcpy(ivec, iv.c_str(), 8);
	DES_ede3_cbc_encrypt(input, output, dataLen, &ks1, &ks2, &ks3, (DES_cblock *)ivec, DES_DECRYPT);

	string plainData;
	if (dataLen > 0)
	{
		string result((char *)output);
		plainData = result;
	}
	return plainData;
}

// 2018.09.13 end

char* TRIPLE_CBC_Encrypt(char* source, char* key, char* cbc_IV)
{
	string strHex = DES_TRIPLE_CBC_Encrypt(key, source, cbc_IV);
	char* pEnd = ConstChar2Char(strHex.c_str());
	return pEnd;
}

char* TRIPLE_CBC_Decrypt(char* source, char* key, char* cbc_IV)
{
	//	string str = DES_TRIPLE_CBC_Decrypt(source, key, cbc_IV);
	string strSource = hex2str(source);
	string str = DES_TRIPLE_CBC_Decrypt(key, strSource, cbc_IV);
	char* pEnd = ConstChar2Char(str.c_str());
	return pEnd;
}

string DES_TRIPLE_ECB_Encrypt(const string cleartext, const string key)
{
	string strCipherText;

	DES_cblock ke1, ke2, ke3;
	memset(ke1, 0, 8);
	memset(ke2, 0, 8);
	memset(ke2, 0, 8);

	if (key.length() >= 24) {
		memcpy(ke1, key.c_str(), 8);
		memcpy(ke2, key.c_str() + 8, 8);
		memcpy(ke3, key.c_str() + 16, 8);
	}
	else if (key.length() >= 16) {
		memcpy(ke1, key.c_str(), 8);
		memcpy(ke2, key.c_str() + 8, 8);
		memcpy(ke3, key.c_str() + 16, key.length() - 16);
	}
	else if (key.length() >= 8) {
		memcpy(ke1, key.c_str(), 8);
		memcpy(ke2, key.c_str() + 8, key.length() - 8);
		memcpy(ke3, key.c_str(), 8);
	}
	else {
		memcpy(ke1, key.c_str(), key.length());
		memcpy(ke2, key.c_str(), key.length());
		memcpy(ke3, key.c_str(), key.length());
	}

	DES_key_schedule ks1, ks2, ks3;
	DES_set_key_unchecked(&ke1, &ks1);
	DES_set_key_unchecked(&ke2, &ks2);
	DES_set_key_unchecked(&ke3, &ks3);

	const_DES_cblock inputText;
	DES_cblock outputText;
	vector<unsigned char> vecCiphertext;
	unsigned char tmp[8];

	for (int i = 0; i < cleartext.length() / 8; i++) {
		memcpy(inputText, cleartext.c_str() + i * 8, 8);
		DES_ecb3_encrypt(&inputText, &outputText, &ks1, &ks2, &ks3, DES_ENCRYPT);
		memcpy(tmp, outputText, 8);

		for (int j = 0; j < 8; j++)
		{
			string strHex = Uchar2HEX(tmp[j]);
			vecCiphertext.push_back(strHex[0]);
			vecCiphertext.push_back(strHex[1]);
		}
			
	}

	if (cleartext.length() % 8 != 0) {
		int tmp1 = cleartext.length() / 8 * 8;
		int tmp2 = cleartext.length() - tmp1;
		memset(inputText, 0, 8);
		memcpy(inputText, cleartext.c_str() + tmp1, tmp2);

		DES_ecb3_encrypt(&inputText, &outputText, &ks1, &ks2, &ks3, DES_ENCRYPT);
		memcpy(tmp, outputText, 8);

		for (int j = 0; j < 8; j++)
		{
			string strHex = Uchar2HEX(tmp[j]);
			vecCiphertext.push_back(strHex[0]);
			vecCiphertext.push_back(strHex[1]);
		}
	}

	strCipherText.clear();
	strCipherText.assign(vecCiphertext.begin(), vecCiphertext.end());

	return strCipherText;
}

string DES_TRIPLE_ECB_Decrypt(const string ciphertext, const string key)
{
	string strClearText;

	DES_cblock ke1, ke2, ke3;
	memset(ke1, 0, 8);
	memset(ke2, 0, 8);
	memset(ke2, 0, 8);

	if (key.length() >= 24) {
		memcpy(ke1, key.c_str(), 8);
		memcpy(ke2, key.c_str() + 8, 8);
		memcpy(ke3, key.c_str() + 16, 8);
	}
	else if (key.length() >= 16) {
		memcpy(ke1, key.c_str(), 8);
		memcpy(ke2, key.c_str() + 8, 8);
		memcpy(ke3, key.c_str() + 16, key.length() - 16);
	}
	else if (key.length() >= 8) {
		memcpy(ke1, key.c_str(), 8);
		memcpy(ke2, key.c_str() + 8, key.length() - 8);
		memcpy(ke3, key.c_str(), 8);
	}
	else {
		memcpy(ke1, key.c_str(), key.length());
		memcpy(ke2, key.c_str(), key.length());
		memcpy(ke3, key.c_str(), key.length());
	}

	DES_key_schedule ks1, ks2, ks3;
	DES_set_key_unchecked(&ke1, &ks1);
	DES_set_key_unchecked(&ke2, &ks2);
	DES_set_key_unchecked(&ke3, &ks3);

	const_DES_cblock inputText;
	DES_cblock outputText;
	vector<unsigned char> vecCleartext;
	unsigned char tmp[8];

	for (int i = 0; i < ciphertext.length() / 8; i++) {
		memcpy(inputText, ciphertext.c_str() + i * 8, 8);
		DES_ecb3_encrypt(&inputText, &outputText, &ks1, &ks2, &ks3, DES_DECRYPT);
		memcpy(tmp, outputText, 8);

		for (int j = 0; j < 8; j++)
			vecCleartext.push_back(tmp[j]);
	}

	if (ciphertext.length() % 8 != 0) {
		int tmp1 = ciphertext.length() / 8 * 8;
		int tmp2 = ciphertext.length() - tmp1;
		memset(inputText, 0, 8);
		memcpy(inputText, ciphertext.c_str() + tmp1, tmp2);

		DES_ecb3_encrypt(&inputText, &outputText, &ks1, &ks2, &ks3, DES_DECRYPT);
		memcpy(tmp, outputText, 8);

		for (int j = 0; j < 8; j++)
			vecCleartext.push_back(tmp[j]);
	}

	strClearText.clear();
	strClearText.assign(vecCleartext.begin(), vecCleartext.end());

	return strClearText;
}

char* TRIPLE_ECB_Encrypt(char* source, char* key)
{
	string strHex = DES_TRIPLE_ECB_Encrypt(source, key);
	char* pEnd = ConstChar2Char(strHex.c_str());
	return pEnd;
}

char* TRIPLE_ECB_Decrypt(char* source, char* key)
{
	string strSource = hex2str(source);
	string str = DES_TRIPLE_ECB_Decrypt(strSource, key);
	char* pEnd = ConstChar2Char(str.c_str());
	return pEnd;
}

char* TRIPLE_ECB_EncryptDefaultKey(char* source)
{
	return TRIPLE_ECB_Encrypt(source, keyDefault);
}

char* TRIPLE_ECB_DecryptDefaultKey(char* source)
{
	return TRIPLE_ECB_Decrypt(source, keyDefault);
}

char* TRIPLE_CBC_EncryptDefaultKeyIV(char* source)
{
	return TRIPLE_CBC_Encrypt(source, keyDefault, IVDefault);
}

char* TRIPLE_CBC_DecryptDefaultKeyIV(char* source)
{
	return TRIPLE_CBC_Decrypt(source, keyDefault, IVDefault);
}



// add by fdp 181022 add RSA encryptor
char Dec2HexChar(short int n)
{
	if (0 <= n && n <= 9) {
		return char(short('0') + n);
	}
	else if (10 <= n && n <= 15) {
		return char(short('A') + n - 10);
	}
	else {
		return char(0);
	}
}

short int HexChar2Dec(char c)
{
	if ('0' <= c && c <= '9') {
		return short(c - '0');
	}
	else if ('a' <= c && c <= 'f') {
		return (short(c - 'a') + 10);
	}
	else if ('A' <= c && c <= 'F') {
		return (short(c - 'A') + 10);
	}
	else {
		return -1;
	}
}

std::string EncodeURL(const std::string &URL)
{
	std::string strResult = "";
	for (unsigned int i = 0; i < URL.size(); i++)
	{
		char c = URL[i];
		if (
			('0' <= c && c <= '9') ||
			('a' <= c && c <= 'z') ||
			('A' <= c && c <= 'Z') ||
			c == '.'
			) {
			strResult += c;
		}
		else
		{
			int j = (short int)c;
			if (j < 0)
			{
				j += 256;
			}
			int i1, i0;
			i1 = j / 16;
			i0 = j - i1 * 16;
			strResult += '%';
			strResult += Dec2HexChar(i1);
			strResult += Dec2HexChar(i0);
		}
	}

	return strResult;
}

std::string DecodeURL(const std::string &URL)
{
	std::string result = "";
	for (unsigned int i = 0; i < URL.size(); i++)
	{
		char c = URL[i];
		if (c != '%')
		{
			result += c;
		}
		else
		{
			char c1 = URL[++i];
			char c0 = URL[++i];
			int num = 0;
			num += HexChar2Dec(c1) * 16 + HexChar2Dec(c0);
			result += char(num);
		}
	}

	return result;
}

//--生成GUID    
//const char* newGUID()
//{
//	static char buf[64] = { 0 };
//	GUID guid;
//	if (S_OK == ::CoCreateGuid(&guid))
//	{
//		_snprintf(buf, sizeof(buf)
//			, "%08X%04X%04X%02X%02X%02X%02X%02X%02X%02X%02X"
//			, guid.Data1
//			, guid.Data2
//			, guid.Data3
//			, guid.Data4[0], guid.Data4[1]
//			, guid.Data4[2], guid.Data4[3], guid.Data4[4], guid.Data4[5]
//			, guid.Data4[6], guid.Data4[7]
//			);
//	}
//	return (const char*)buf;
//}





//将二进制流转换成base64编码  
const char * base64char = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
char * base64_encode(const unsigned char * bindata, char * base64, int binlength)
{
	int i, j;
	unsigned char current;

	for (i = 0, j = 0; i < binlength; i += 3)
	{
		current = (bindata[i] >> 2);
		current &= (unsigned char)0x3F;
		base64[j++] = base64char[(int)current];

		current = ((unsigned char)(bindata[i] << 4)) & ((unsigned char)0x30);
		if (i + 1 >= binlength)
		{
			base64[j++] = base64char[(int)current];
			base64[j++] = '=';
			base64[j++] = '=';
			break;
		}
		current |= ((unsigned char)(bindata[i + 1] >> 4)) & ((unsigned char)0x0F);
		base64[j++] = base64char[(int)current];

		current = ((unsigned char)(bindata[i + 1] << 2)) & ((unsigned char)0x3C);
		if (i + 2 >= binlength)
		{
			base64[j++] = base64char[(int)current];
			base64[j++] = '=';
			break;
		}
		current |= ((unsigned char)(bindata[i + 2] >> 6)) & ((unsigned char)0x03);
		base64[j++] = base64char[(int)current];

		current = ((unsigned char)bindata[i + 2]) & ((unsigned char)0x3F);
		base64[j++] = base64char[(int)current];
	}
	base64[j] = '\0';
	return base64;
}

void md5(const std::string &srcStr, std::string &encodedStr, std::string &encodedHexStr)
{
	// 调用md5哈希    
	unsigned char mdStr[33] = { 0 };
	MD5((const unsigned char *)srcStr.c_str(), srcStr.length(), mdStr);

	// 哈希后的字符串    
	encodedStr = std::string((const char *)mdStr);
	// 哈希后的十六进制串 32字节    
	char buf[65] = { 0 };
	char tmp[3] = { 0 };
	for (int i = 0; i < 32; i++)
	{
		sprintf(tmp, "%02x", mdStr[i]);
		strcat(buf, tmp);
	}
	buf[32] = '\0'; // 后面都是0，从32字节截断    
	encodedHexStr = std::string(buf);
}

char* sha256(char* pcOrigana, int iLength )
{
	char buf[2];
	unsigned char hash[SHA256_DIGEST_LENGTH];
	SHA256_CTX sha256;
	SHA256_Init(&sha256);
	SHA256_Update(&sha256, pcOrigana, iLength);
	SHA256_Final(hash, &sha256);

	string strtemp = string((char*)hash, SHA256_DIGEST_LENGTH);
	string strhash = str2hex(strtemp, true);

	char* pEnd = ConstChar2Char(strhash.c_str());
	return pEnd;
}





// 函数方法生成密钥对   
void generateRSAKey(std::string strKey[2])
{
	// 公私密钥对    
	size_t pri_len;
	size_t pub_len;
	char *pri_key = NULL;
	char *pub_key = NULL;

	// 生成密钥对    
	RSA *keypair = RSA_generate_key(KEY_LENGTH, RSA_3, NULL, NULL);

	BIO *pri = BIO_new(BIO_s_mem());
	BIO *pub = BIO_new(BIO_s_mem());

	PEM_write_bio_RSAPrivateKey(pri, keypair, NULL, NULL, 0, NULL, NULL);
	PEM_write_bio_RSAPublicKey(pub, keypair);

	// 获取长度    
	pri_len = BIO_pending(pri);
	pub_len = BIO_pending(pub);

	// 密钥对读取到字符串    
	pri_key = (char *)malloc(pri_len + 1);
	pub_key = (char *)malloc(pub_len + 1);

	BIO_read(pri, pri_key, pri_len);
	BIO_read(pub, pub_key, pub_len);

	pri_key[pri_len] = '\0';
	pub_key[pub_len] = '\0';

	// 存储密钥对    
	strKey[0] = pub_key;
	strKey[1] = pri_key;

	// 存储到磁盘（这种方式存储的是begin rsa public key/ begin rsa private key开头的）  
	FILE *pubFile = fopen(PUB_KEY_FILE, "w");
	if (pubFile == NULL)
	{
	
		return;
	}
	fputs(pub_key, pubFile);
	fclose(pubFile);

	FILE *priFile = fopen(PRI_KEY_FILE, "w");
	if (priFile == NULL)
	{
		
		return;
	}
	fputs(pri_key, priFile);
	fclose(priFile);

	// 内存释放  
	RSA_free(keypair);
	BIO_free_all(pub);
	BIO_free_all(pri);

	free(pri_key);
	free(pub_key);
}

void generateRSAKey2File(const char* pcPubKeyPath , const char* pcPriKeyPath)
{
	// 公私密钥对    
	size_t pri_len;
	size_t pub_len;
	char *pri_key = NULL;
	char *pub_key = NULL;

	// 生成密钥对    
	RSA *keypair = RSA_generate_key(KEY_LENGTH, RSA_3, NULL, NULL);

	BIO *pri = BIO_new(BIO_s_mem());
	BIO *pub = BIO_new(BIO_s_mem());

	PEM_write_bio_RSAPrivateKey(pri, keypair, NULL, NULL, 0, NULL, NULL);
	PEM_write_bio_RSAPublicKey(pub, keypair);

	// 获取长度    
	pri_len = BIO_pending(pri);
	pub_len = BIO_pending(pub);

	// 密钥对读取到字符串    
	pri_key = (char *)malloc(pri_len + 1);
	pub_key = (char *)malloc(pub_len + 1);

	BIO_read(pri, pri_key, pri_len);
	BIO_read(pub, pub_key, pub_len);

	pri_key[pri_len] = '\0';
	pub_key[pub_len] = '\0';

	// 存储到磁盘（这种方式存储的是begin rsa public key/ begin rsa private key开头的）  
	FILE *pubFile = fopen(pcPubKeyPath, "w");
	if (pubFile == NULL)
	{
		return;
	}
	fputs(pub_key, pubFile);
	fclose(pubFile);

	FILE *priFile = fopen(pcPriKeyPath, "w");
	if (priFile == NULL)
	{

		return;
	}
	fputs(pri_key, priFile);
	fclose(priFile);

	// 内存释放  
	RSA_free(keypair);
	BIO_free_all(pub);
	BIO_free_all(pri);

	free(pri_key);
	free(pub_key);
}

// 生成公钥文件和私钥文件，私钥文件带密码
int generate_key_files(const char *pub_keyfile, const char *pri_keyfile,
	const unsigned char *passwd, int passwd_len)
{
	RSA *rsa = NULL;
	//RAND_seed(rnd_seed, sizeof(rnd_seed));
	rsa = RSA_generate_key(KEY_LENGTH, RSA_F4, NULL, NULL);
	if (rsa == NULL)
	{
		printf("RSA_generate_key error!\n");
		return -1;
	}

	// 开始生成公钥文件
	BIO *bp = BIO_new(BIO_s_file());
	if (NULL == bp)
	{
		printf("generate_key bio file new error!\n");
		return -1;
	}

	if (BIO_write_filename(bp, (void *)pub_keyfile) <= 0)
	{
		printf("BIO_write_filename error!\n");
		return -1;
	}

	if (PEM_write_bio_RSAPublicKey(bp, rsa) != 1)
	{
		printf("PEM_write_bio_RSAPublicKey error!\n");
		return -1;
	}

	// 公钥文件生成成功，释放资源
	printf("Create public key ok!\n");
	BIO_free_all(bp);

	// 生成私钥文件
	bp = BIO_new_file(pri_keyfile, "w+");
	if (NULL == bp)
	{
		printf("generate_key bio file new error2!\n");
		return -1;
	}

	if (PEM_write_bio_RSAPrivateKey(bp, rsa,
		EVP_des_ede3_ofb(), (unsigned char *)passwd,
		passwd_len, NULL, NULL) != 1)
	{
		printf("PEM_write_bio_RSAPublicKey error!\n");
		return -1;
	}

	// 释放资源
	printf("Create private key ok!\n");
	BIO_free_all(bp);
	RSA_free(rsa);

	return 0;
}




// 打开公钥文件，返回EVP_PKEY结构的指针
EVP_PKEY* open_public_key(const char *keyfile)
{
	EVP_PKEY* key = NULL;
	RSA *rsa = NULL;

	OpenSSL_add_all_algorithms();
	BIO *bp = BIO_new(BIO_s_file());;
	BIO_read_filename(bp, keyfile);
	if (NULL == bp)
	{
		printf("open_public_key bio file new error!\n");
		return NULL;
	}

	rsa = PEM_read_bio_RSAPublicKey(bp, NULL, NULL, NULL);
	if (rsa == NULL)
	{
		printf("open_public_key failed to PEM_read_bio_RSAPublicKey!\n");
		BIO_free(bp);
		RSA_free(rsa);

		return NULL;
	}

	printf("open_public_key success to PEM_read_bio_RSAPublicKey!\n");
	key = EVP_PKEY_new();
	if (NULL == key)
	{
		printf("open_public_key EVP_PKEY_new failed\n");
		RSA_free(rsa);

		return NULL;
	}

	EVP_PKEY_assign_RSA(key, rsa);
	return key;
}

// 打开私钥文件，返回EVP_PKEY结构的指针
EVP_PKEY* open_private_key(const char *keyfile, const unsigned char *passwd)
{
	EVP_PKEY* key = NULL;
	RSA *rsa = RSA_new();
	OpenSSL_add_all_algorithms();
	BIO *bp = NULL;
	bp = BIO_new_file(keyfile, "rb");
	if (NULL == bp)
	{
		printf("open_private_key bio file new error!\n");

		return NULL;
	}

	rsa = PEM_read_bio_RSAPrivateKey(bp, &rsa, NULL, (void *)passwd);
	if (rsa == NULL)
	{
		printf("open_private_key failed to PEM_read_bio_RSAPrivateKey!\n");
		BIO_free(bp);
		RSA_free(rsa);

		return NULL;
	}

	printf("open_private_key success to PEM_read_bio_RSAPrivateKey!\n");
	key = EVP_PKEY_new();
	if (NULL == key)
	{
		printf("open_private_key EVP_PKEY_new failed\n");
		RSA_free(rsa);

		return NULL;
	}

	EVP_PKEY_assign_RSA(key, rsa);
	return key;
}


// 公钥加密
char* rsa_pub_encrypt(const char* clearText, int iTxtlen, const char* pubKey, bool bIsKeybyPath)
{
	std::string strRet;
	RSA *rsa = NULL;
	BIO *keybio = NULL;
	if (bIsKeybyPath)
	{
		keybio = BIO_new_file(pubKey, "r");
	}
	else
	{
		keybio =  BIO_new_mem_buf((unsigned char *)pubKey, -1);
	}
	
	rsa = PEM_read_bio_RSAPublicKey(keybio, &rsa, NULL, NULL);

	int len = RSA_size(rsa);

	char *encryptedText = (char *)malloc(len + 1);
	memset(encryptedText, 0, len + 1);

	// 加密函数  
	int ret = RSA_public_encrypt(iTxtlen, (const unsigned char*)clearText, (unsigned char*)encryptedText, rsa, RSA_PKCS1_PADDING);
	if (ret >= 0)
	{
		string strChar = std::string(encryptedText, ret);
		strRet = str2hex(strChar, true);
		//strRet = Memory2hex(encryptText, iRet, true);
		
	}
	// 释放内存  
	free(encryptedText);
	BIO_free_all(keybio);
	RSA_free(rsa);

	return ConstChar2Char(strRet.c_str());
}

// 私钥解密    
//std::string rsa_pri_decrypt(const std::string &cipherText, const std::string &priKey)
char* rsa_pri_decrypt(const char* cipherText, int iTxtlen, const char* priKey, bool bIsKeybyPath)
{
	std::string strRet;
	RSA *rsa = RSA_new();
	BIO *keybio = NULL;
	if (bIsKeybyPath)
	{
		keybio = BIO_new_file(priKey, "r");
	}
	else
	{
		keybio = BIO_new_mem_buf((unsigned char *)priKey, -1);
	}


	rsa = PEM_read_bio_RSAPrivateKey(keybio, &rsa, NULL, NULL);

	int len = RSA_size(rsa);
	char *decryptedText = (char *)malloc(len + 1);
	memset(decryptedText, 0, len + 1);

	string strChar = hex2str(cipherText, iTxtlen);

	// 解密函数  
	//int ret = RSA_private_decrypt(cipherText.length(), (const unsigned char*)cipherText.c_str(), (unsigned char*)decryptedText, rsa, RSA_PKCS1_PADDING);
	int ret = RSA_private_decrypt(strChar.length(), (const unsigned char*)strChar.c_str(), (unsigned char*)decryptedText, rsa, RSA_PKCS1_PADDING);
	if (ret >= 0)
		strRet = std::string(decryptedText, ret);

	// 释放内存  
	free(decryptedText);
	BIO_free_all(keybio);
	RSA_free(rsa);

	return ConstChar2Char(strRet.c_str());
}

// 通过prikey 值进行计算
//std::string rsa_pri_sign(const std::string &strText, const std::string &priKey)
char* rsa_pri_sign(const char* pcText, int iTxtlen, const char* pcPriKey, bool bIsKeybyPath)
{
	string strRet = "";
	RSA *rsa = RSA_new();
	BIO *keybio=NULL;
	if (bIsKeybyPath)
	{
		keybio = BIO_new_file(pcPriKey, "r");
	}
	else
	{
		keybio = BIO_new_mem_buf((unsigned char *)pcPriKey, -1);
	}

	rsa = PEM_read_bio_RSAPrivateKey(keybio, &rsa, NULL, NULL);

	int len = RSA_size(rsa);
	char *encryptText = (char*)malloc( len +1 );
	memset(encryptText,0, len + 1);
	int iRet = RSA_private_encrypt(iTxtlen, (const unsigned char*)pcText, (unsigned char*)encryptText, rsa, RSA_PKCS1_PADDING);
	if (iRet > 0 )
	{
		//strRet = str2hex(encryptText, true);
		strRet = Memory2hex(encryptText, iRet, true);
	}
	free(encryptText);
	BIO_free_all(keybio);
	RSA_free(rsa);
	return ConstChar2Char(strRet.c_str());
}

//std::string rsa_pri_sign_byKeyPath(const std::string &strText, const std::string &priKeyPath)
char* rsa_pub_DecryptSignature(const char* pcText, int iTxtlen, const char* pubKey, bool bIsKeybyPath)
{
	string strRet = "";
	RSA *rsa = RSA_new();
	BIO *keybio = NULL;
	if (bIsKeybyPath)
	{
		keybio = BIO_new_file(pubKey, "r");
	}
	else
	{
		keybio = BIO_new_mem_buf((unsigned char *)pubKey, -1);
	}
	
	rsa = PEM_read_bio_RSAPublicKey(keybio, &rsa, NULL, NULL);

	int len = RSA_size(rsa);
	char *encryptText = (char*)malloc(len + 1);
	memset(encryptText, 0, len + 1);
	string strChar = hex2str(pcText, iTxtlen);
	int iRet = RSA_public_decrypt(strChar.length(), (const unsigned char*)strChar.c_str(), (unsigned char*)encryptText, rsa, RSA_PKCS1_PADDING);
	if (iRet > 0)
	{
		//strRet = str2hex(encryptText, true);
		strRet = std::string(encryptText, iRet);
		//strRet = Memory2hex(encryptText, iRet, true);
	}
	free(encryptText);
	BIO_free_all(keybio);
	RSA_free(rsa);
	return ConstChar2Char(strRet.c_str());
}


std::string rsa_pub_checkSignature(const std::string &strText, const std::string &pubKey)
{
	string strRet = "";
	RSA *rsa =RSA_new();
	BIO *keynio = NULL;

	
	keynio = BIO_new_mem_buf((unsigned char *)pubKey.c_str(), -1);
	rsa = PEM_read_bio_RSAPublicKey(keynio, &rsa, NULL, NULL);
	int len = RSA_size(rsa);



	char *encryptText = (char*)malloc(len + 1);
	memset(encryptText, 0, len + 1);

	string strChar = hex2str(strText);
	//int iRet = RSA_public_decrypt(strText.length(), (const unsigned char*)strText.c_str(), (unsigned char*)encryptText, rsa, RSA_PKCS1_PADDING);
	int iRet = RSA_public_decrypt(strChar.length(), (const unsigned char*)strChar.c_str(), (unsigned char*)encryptText, rsa, RSA_PKCS1_PADDING);
	if (iRet > 0)
	{
		strRet = std::string(encryptText, iRet);
	}
	free(encryptText);
	BIO_free_all(keynio);
	RSA_free(rsa);
	return strRet;
}

std::string rsa_pub_checkSignature_byKeyPath(const std::string &strText, const std::string &pubKeyPath)
{
	string strRet = "";
	RSA *rsa = RSA_new();
	BIO *keynio = NULL;
	keynio = BIO_new_file(pubKeyPath.c_str(), "r");
	rsa = PEM_read_bio_RSAPublicKey(keynio, &rsa, NULL, NULL);
	int len = RSA_size(rsa);



	char *encryptText = (char*)malloc(len + 1);
	memset(encryptText, 0, len + 1);

	string strChar = hex2str(strText);
	//int iRet = RSA_public_decrypt(strText.length(), (const unsigned char*)strText.c_str(), (unsigned char*)encryptText, rsa, RSA_PKCS1_PADDING);
	int iRet = RSA_public_decrypt(strChar.length(), (const unsigned char*)strChar.c_str(), (unsigned char*)encryptText, rsa, RSA_PKCS1_PADDING);
	if (iRet > 0)
	{
		strRet = std::string(encryptText, iRet);
	}
	free(encryptText);
	BIO_free_all(keynio);
	RSA_free(rsa);
	return strRet;
}
/* generaCSR
void generaCSR()
{
	X509 *x509 = NULL;
	X509_NAME *subject = NULL;
	BIO *bio = NULL;
	X509_REQ *x509Req = NULL;
	char *szCSR = NULL;
	// 提取私钥
	EVP_PKEY_assign_RSA(m_pKey, m_rsa);

	x509 = X509_new();
	X509_set_pubkey(x509, m_pKey);

	// 设置属性
	subject = X509_get_subject_name(x509);
	// 国家
	X509_NAME_add_entry_by_txt(subject, SN_countryName, MBSTRING_UTF8,
		(unsigned char *)"CN", -1, -1, 0);
	// 省份
	X509_NAME_add_entry_by_txt(subject, SN_stateOrProvinceName, MBSTRING_UTF8,
		(unsigned char *)"GuangDong", -1, -1, 0);
	// 城市
	X509_NAME_add_entry_by_txt(subject, SN_localityName, MBSTRING_UTF8,
		(unsigned char *)"ShenZhen", -1, -1, 0);

	X509_set_subject_name(x509, subject);



	//X509 *ptemp = NULL;
	//X509 *m_pClientCert;
	//m_pClientCert = X509_new();
	////设置版本号
	//X509_set_version(ptemp, 2);
	////设置证书序列号，这个sn就是CA中心颁发的第N份证书
	//ASN1_INTEGER_set(X509_get_serialNumber(ptemp),1);
	////设置证书开始时间
	//X509_gmtime_adj(X509_get_notBefore(ptemp), 0);
	////设置证书结束时间
	//X509_gmtime_adj(X509_get_notAfter(ptemp), (long)60 * 60 * 24 * 10);
	////设置证书的主体名称，req就是刚刚生成的请求证书
	//X509_set_subject_name(ptemp, X509_REQ_get_subject_name(x509Req));
	////设置证书的公钥信息
	//X509_set_pubkey(ptemp, X509_PUBKEY_get(x509Req->req_info->pubkey));
	//设置证书的签发者信息，m_pCACert是CA证书
	//X509_set_issuer_name(ptemp, X509_get_subject_name(m_pCACert));
	//设置扩展项目
	//X509V3_CTX ctx;
	//X509V3_set_ctx(&ctx, m_pCACert, m_pClientCert, NULL, NULL, 0);
	//X509_EXTENSION *x509_ext = X509_EXTENSION_new();
	//x509_ext = X509V3_EXT_conf(NULL, name，value);
	//X509_add_ext(m_pClientCert, x509_ext, -1);
	////设置签名值
	//X509_sign(m_pClientCert, m_pCAKey, EVP_md5());
	////这样一份X509证书就生成了，下面的任务就是对它进行编码保存。
	//i2d_X509_bio(pbio, m_pClientCert);// DER格式
	//	PEM_write_bio_X509(pbio, m_pClientCert);// PEM格式



	x509Req = X509_to_X509_REQ(x509, m_pKey, EVP_md5());
	if (!x509Req)
	{
		goto free_all;
	}

	// 可视化输出
	bio = BIO_new(BIO_s_mem());
	PEM_write_bio_X509_REQ(bio, x509Req);
	if (bio->num_write == 0)
	{
		goto free_all;
	}

	szCSR = (char*)malloc(bio->num_write + 1);
	if (!szCSR)
	{
		goto free_all;
	}

	memset(szCSR, 0, bio->num_write + 1);
	BIO_read(bio, szCSR, bio->num_write);


free_all:
	if (x509)
		X509_free(x509);
	if (x509Req)
		X509_REQ_free(x509Req);
	if (bio)
		BIO_free(bio);
	if (szCSR)
		free(szCSR);


}
*/

//int generaCSR_1()
//{
//	X509_REQ *req;
//	int ret;
//	long version;
//
//	X509_NAME *name;
//	EVP_PKEY *pkey;
//	RSA *rsa;
//	X509_NAME_ENTRY   *entry = NULL;
//	char bytes[100], mdout[20];
//	unsigned int len, mdlen;
//	int bits = 1024;
//	//unsigned long  e = RSA_3;
//	unsigned long  e = RSA_F4;
//	unsigned char*der, *p;
//	FILE *fp;
//	const EVP_MD *md;
//	X509  *x509;
//	BIO *b;
//	STACK_OF(X509_EXTENSION) *exts;
//	req = X509_REQ_new();
//
//	version = 1;
//	ret = X509_REQ_set_version(req, version);
//	name = X509_NAME_new();
//	strcpy(bytes, "openssl");
//	len = strlen(bytes);
//	entry = X509_NAME_ENTRY_create_by_txt(&entry, "commonName", V_ASN1_UTF8STRING, (unsigned char *)bytes, len);
//	X509_NAME_add_entry(name, entry, 0, -1);
//	strcpy(bytes, "bj");
//	len = strlen(bytes);
//	entry = X509_NAME_ENTRY_create_by_txt(&entry, "countryName", V_ASN1_UTF8STRING, (const unsigned char *)bytes, len);
//	X509_NAME_add_entry(name, entry, 1, -1);
//
//
//	// 国家
//	X509_NAME_add_entry_by_txt(name, SN_countryName, MBSTRING_UTF8,
//		(unsigned char *)"CN", -1, -1, 0);
//	// 省份
//	X509_NAME_add_entry_by_txt(name, SN_stateOrProvinceName, MBSTRING_UTF8,
//		(unsigned char *)"GuangDong", -1, -1, 0);
//	// 城市
//	X509_NAME_add_entry_by_txt(name, SN_localityName, MBSTRING_UTF8,
//		(unsigned char *)"ShenZhen", -1, -1, 0);
//
//
//
//	/* subject name */
//
//	ret = X509_REQ_set_subject_name(req, name);
//
//	/* pub key */
//
//	pkey = EVP_PKEY_new();
//
//	rsa = RSA_generate_key(bits, e, NULL, NULL);
//
//	EVP_PKEY_assign_RSA(pkey, rsa);
//
//	ret = X509_REQ_set_pubkey(req, pkey);
//
//	/* attribute */
//
//	strcpy(bytes, "test");
//
//	len = strlen(bytes);
//
//	ret = X509_REQ_add1_attr_by_txt(req, "organizationName", V_ASN1_UTF8STRING, (const unsigned char*)bytes, len);
//
//	strcpy(bytes, "ttt");
//
//	len = strlen(bytes);
//
//	ret = X509_REQ_add1_attr_by_txt(req, "organizationalUnitName", V_ASN1_UTF8STRING, (const unsigned char*)bytes, len);
//
//	md = EVP_sha1();
//
//	ret = X509_REQ_digest(req, md, ( unsigned char*)mdout, &mdlen);
//
//	ret = X509_REQ_sign(req, pkey, md);
//
//	if (!ret)
//
//	{
//
//		printf("sign err!\n");
//
//		X509_REQ_free(req);
//
//		return -1;
//
//	}
//
//	/* 写入文件PEM格式 */
//
//	b = BIO_new_file("certreq.txt", "w");
//
//	PEM_write_bio_X509_REQ(b, req);
//
//	BIO_free(b);
//
//	/* DER编码 */
//
//	len = i2d_X509_REQ(req, NULL);
//
//	der = (unsigned char*)malloc(len);
//
//	p = der;
//
//	len = i2d_X509_REQ(req, &p);
//
//	OpenSSL_add_all_algorithms();
//
//	ret = X509_REQ_verify(req, pkey);
//
//	if (ret < 0)
//
//	{
//
//		printf("verify err.\n");
//
//	}
//
//	fp = fopen("certreq2.txt", "wb");
//
//	fwrite(der, 1, len, fp);
//	
//	
//	//test begin
//	X509 *ptemp = X509_new();
//	X509 *m_pClientCert;
//	m_pClientCert = X509_new();
//
//	X509_set_version(ptemp, 2);
//
//	ASN1_INTEGER_set(X509_get_serialNumber(ptemp), 100);
//
//	X509_gmtime_adj(X509_get_notBefore(ptemp), 0);
//
//	X509_gmtime_adj(X509_get_notAfter(ptemp), (long)60 * 60 * 24 * 100);
//
//	X509_set_subject_name(ptemp, X509_REQ_get_subject_name(req));
//
//	X509_set_pubkey(ptemp, X509_PUBKEY_get(req->req_info->pubkey));
//
//	X509_set_issuer_name(ptemp, X509_get_subject_name(m_pCACert));
//
//	m_pClientCert = ptemp;
//	X509V3_CTX ctx;
//	X509V3_set_ctx(&ctx, m_pCACert, m_pClientCert, NULL, NULL, 0);
//
//	X509_EXTENSION *x509_ext = X509_EXTENSION_new();
//	string name_ext = "fan";
//	string value_ext = "test";
//
//	x509_ext = X509V3_EXT_conf(NULL, &ctx, (char*)name_ext.c_str(),(char*)value_ext.c_str());
//	X509_add_ext(m_pClientCert, x509_ext, -1);
//	X509_sign(m_pClientCert, pkey, EVP_md5());
//	//BIO *pbio = new BIO();
//	BIO *pbio = BIO_new_file("certreq3.txt", "w");
//	
//	//i2d_X509_bio(pbio, m_pClientCert);
//	
//	PEM_write_bio_X509(pbio, m_pClientCert);
//
//	//b = BIO_new_file("certreq.txt", "w");
//
//	//PEM_write_bio_X509_REQ(b, req);
//
//	BIO_free(pbio);
//
//	//end
//	fclose(fp);
//
//	free(der);
//
//	X509_REQ_free(req);
//
//	return 0;
//
//}

int CSR2X509(X509_REQ **req)
{
	BIO *in;
	//X509_REQ *req = NULL;
	//X509_REQ **req2 = NULL;
	FILE *fp;
	int len;

	in = BIO_new_file("certreq.txt", "r");
	*req = PEM_read_bio_X509_REQ(in, NULL, NULL, NULL);

	if (req == NULL)
	{
		printf("pem解码错误!\n");
	}
	else
	{
		printf("pem解码成功!\n");
	}

free_all:
	//if (req)
	//	X509_REQ_free(req);
	//if (req2)
	//{
	//	X509_REQ_free(*req2);
	//}
	if (in)
		BIO_free(in);

	return 0;
}


// 证书转化为X509结构
void x509FromCertString(string cert, X509 **pCACert)//, X509 **pX509)
{
	if (cert.length() == 0)
		return;

	BIO *bio = NULL;

	const char *certData = cert.c_str();
	bio = BIO_new_file(cert.c_str(), "r");
	if (bio == NULL)
	{
		printf("can not open b64cert.cer!\n");
	}

	*pCACert = PEM_read_bio_X509(bio, NULL, NULL, NULL);

	BIO_free(bio);
}

bool checkX509Data(X509 *x509, EVP_PKEY *pKey)
{
	// 校验密钥和证书是否匹配
	if (!X509_check_private_key(x509, pKey))
	{
		return false;
	}
	X509 *m_rootCert = NULL;
	// 根证书校验
	if (m_rootCert == NULL)
	{
		BIO *bio = BIO_new_file("HSBC_Root_CA.cer", "r");
		if (!bio)
		{
			return false;
		}

		PEM_read_bio_X509(bio, &m_rootCert, NULL, NULL);

		BIO_free(bio);

		if (m_rootCert == NULL)
		{
			return false;
		}
	}

	EVP_PKEY *pubKey = X509_get_pubkey(m_rootCert);
	if (X509_verify(x509, pubKey) != 1)
	{
		return false;
	}

	return true;
}


/*
void importCert(string cert, string path)
{
	if (cert.length() == 0 || path.length() == 0)
		return;

	int ret = 0;
	X509 *x509 = NULL;
	BIO *bioW = NULL;

	ret = x509FromCertString(cert, &x509);
	if (ret)
	{
		goto free_all;
	}

	ret = checkX509Data(x509, false);
	if (ret)
	{
		goto free_all;
	}

	bioW = BIO_new_file(path.c_str(), "wb");
	if (!bioW)
	{
		goto free_all;
	}

	// 写入文件
	if (i2d_X509_bio(bioW, x509) != 1)
	{
		goto free_all;
	}


free_all:
	if (x509)
		X509_free(x509);
	if (bioW)
		BIO_free(bioW);

}
*/

// 生成CSR文件
int generaCSRFile(string strCSRFile, int iRSALen, int iVersion, unsigned long ulMod, string strcommonName,
				  string strcountryName, string strproviceName, string strcityName, string strorganizationName,
				  string strorganizationalUnitName, bool bIsOutPem)
{
	X509_REQ *req;
	int ret;
	X509_NAME *x509Name;
	RSA *rsa;
	X509_NAME_ENTRY  *entry = NULL;
	char  mdout[20];
	unsigned int len, mdlen;
	int bits = iRSALen;

	unsigned char*der, *p;
	FILE *fp;
	const EVP_MD *md;
	//X509  *x509;
	BIO *b;
	STACK_OF(X509_EXTENSION) *exts;

	req = X509_REQ_new();
	ret = X509_REQ_set_version(req, iVersion);
	
	x509Name = X509_NAME_new();
	
	entry = X509_NAME_ENTRY_create_by_txt(&entry, "commonName", V_ASN1_UTF8STRING, (const unsigned char*)strcommonName.c_str(), strcommonName.length());
	X509_NAME_add_entry(x509Name, entry, 0, -1);

	// nation
	X509_NAME_add_entry_by_txt(x509Name, SN_countryName, MBSTRING_UTF8,(unsigned char *)strcountryName.c_str(), -1, -1, 0);
	// province
	X509_NAME_add_entry_by_txt(x509Name, SN_stateOrProvinceName, MBSTRING_UTF8,(unsigned char *)strproviceName.c_str(), -1, -1, 0);
	// city
	X509_NAME_add_entry_by_txt(x509Name, SN_localityName, MBSTRING_UTF8,(unsigned char *)strcityName.c_str(), -1, -1, 0);

	/* subject name */
	ret = X509_REQ_set_subject_name(req, x509Name);
	/* attribute */
	ret = X509_REQ_add1_attr_by_txt(req, "organizationName", V_ASN1_UTF8STRING, (const unsigned char*)strorganizationName.c_str(), strorganizationName.length());
	ret = X509_REQ_add1_attr_by_txt(req, "organizationName", V_ASN1_UTF8STRING, (const unsigned char*)strorganizationalUnitName.c_str(), strorganizationalUnitName.length());


	
	EVP_PKEY *pKey = EVP_PKEY_new();	
	rsa = RSA_generate_key(bits, ulMod, NULL, NULL);
	EVP_PKEY_assign_RSA(pKey, rsa);

	ret = X509_REQ_set_pubkey(req, pKey);

	//optimize later, digest algorithm should be as a parameter
	md = EVP_sha1();
	ret = X509_REQ_digest(req, md, (unsigned char*)mdout, &mdlen);
	
	// use CSR private sign req structure
	ret = X509_REQ_sign(req, pKey, md);
	if (!ret)
	{
		printf("sign err!\n");
		X509_REQ_free(req);
		return -1;
	}

	//write CSR private key to file 
	BIO *bioPri = BIO_new(BIO_s_mem());
	PEM_write_bio_RSAPrivateKey(bioPri, rsa, NULL, NULL, 0, NULL, NULL);
	int pri_len = BIO_pending(bioPri);
	char *pPri_key = (char *)malloc(pri_len + 1);
	BIO_read(bioPri, pPri_key, pri_len);
	pPri_key[pri_len] = '\0';
	int tempi = strCSRFile.find_last_of('/.');
	string strtempKeypath = strCSRFile;
	strtempKeypath.replace(tempi, strCSRFile.length(), ".key");
	
	FILE *priFile = fopen(strtempKeypath.c_str(), "w");
	if (priFile == NULL)
	{
		return -1;
	}
	fputs(pPri_key, priFile);
	fclose(priFile);



	OpenSSL_add_all_algorithms();
	ret = X509_REQ_verify(req, pKey);
	if (ret < 0)
	{
		printf("verify err.\n");
		return -2;
	}

	if (bIsOutPem)
	{
		b = BIO_new_file(strCSRFile.c_str(), "w");
		PEM_write_bio_X509_REQ(b, req);
		BIO_free(b);
	}
	else
	{
		// write CSR DER file
		len = i2d_X509_REQ(req, NULL);
		der = (unsigned char*)malloc(len);
		p = der;
		len = i2d_X509_REQ(req, &p);
		fp = fopen(strCSRFile.c_str(), "wb");
		fwrite(der, 1, len, fp);
		fclose(fp);
		free(der);
	}

	X509_REQ_free(req);
	return 0;
}

int CSR2X509Req(X509_REQ **req, string strCSRPath)
{
	BIO *in;
	in = BIO_new_file(strCSRPath.c_str(), "r");
	*req = PEM_read_bio_X509_REQ(in, NULL, NULL, NULL);
	if (in)
		BIO_free(in);
	if (req == NULL)
	{
		printf("pem解码错误!\n");
	}
	else
	{
		printf("pem解码成功!\n");
		return 0;
	}

	// pem格式解析失败，尝试解析der格式
	unsigned char buf[1024], *p;
	FILE *fp = fopen(strCSRPath.c_str(), "r");
	int len = fread(buf, 1, 1024, fp);
	fclose(fp);

	p = buf;

	req = (X509_REQ **)malloc(sizeof(X509_REQ *));

	d2i_X509_REQ(req, (const unsigned char**)&p, len);

	if (*req == NULL)
	{
		printf("DER解码错误!\n");
	}

	else

	{
		printf("DER解码成功!\n");
	}

	X509_REQ_free(*req);

	free(req);

	return 0;
}

int getP12Key(const char* cpP12filePath, const char* cpPassword, EVP_PKEY **pkey)
{
	FILE *fp;
	PKCS12 *p12 = NULL;
	PKCS7 *p7 = NULL, *one;
	unsigned char buf[10000], *p;
	int len, i, num, j, count, ret;
	STACK_OF(PKCS7) *p7s;
	STACK_OF(PKCS12_SAFEBAG) *bags;
	PKCS12_SAFEBAG      *bag;
	PBEPARAM            *pbe = 0;
	BIO                 *bp;
	//char                pass[100];

	int passlen;
	X509 *cert = NULL;
	STACK_OF(X509)*ca = NULL;
	//EVP_PKEY *pkey = NULL;
	fp = fopen(cpP12filePath, "rb");
	len = fread(buf, 1, 10000, fp);
	fclose(fp);

	OpenSSL_add_all_algorithms();
	bp = BIO_new(BIO_s_file());
	BIO_set_fp(bp, stdout, BIO_NOCLOSE);
	p = buf;
	d2i_PKCS12(&p12, (const unsigned char **)&p, len);

	ret = PKCS12_parse(p12, cpPassword, pkey, &cert, &ca);
	if (ret != 1)
	{
		printf("err\n");
		return ret;
	}
	/* 私钥写入文件 */
	p = buf;
	len = i2d_PrivateKey(*pkey, &p);
	fp = fopen("prikey.cer", "wb");
	fwrite(buf, 1, len, fp);
	fclose(fp);

	/* 修改密码 */
	//ret = PKCS12_newpass(p12, pass, "test");
	//fp = fopen("newpass.pfx", "wb");
	//ret = i2d_PKCS12_fp(fp, p12);
	//fclose(fp);


	///* version */
	//printf("version : %d\n", ASN1_INTEGER_get(p12->version));
	///*  PKCS12_MAC_DATA */
	//printf("PKCS12_MAC_DATA sig :\n");
	//X509_SIG_print(bp, p12->mac->dinfo);
	//printf("salt : \n");
	//i2a_ASN1_STRING(bp, p12->mac->salt, 1);
	//printf("iter : %d\n", ASN1_INTEGER_get(p12->mac->iter));

	///* p7s */
	//p7s = PKCS12_unpack_authsafes(p12);
	//num = sk_PKCS7_num(p7s);
	//for (i = 0; i < num; i++)
	//{
	//	one = sk_PKCS7_value(p7s, i);
	//	if (PKCS7_type_is_data(one))
	//	{
	//		bags = PKCS12_unpack_p7data(one);
	//		count = sk_PKCS12_SAFEBAG_num(bags);
	//		for (j = 0; j < count; j++)
	//		{
	//			bag = sk_PKCS12_SAFEBAG_value(bags, j);
	//			PKCS12_SAFEBAG_print(bp, bag);
	//		}
	//	}
	//	else if (PKCS7_type_is_encrypted(one))
	//	{
	//	back:
	//		printf("\ninput password :\n");
	//		
	//		passlen = strlen(cpPassword);
	//		bags = PKCS12_unpack_p7encdata(one, cpPassword, passlen);
	//		if (bags == NULL)
	//			goto back;
	//		printf("passwod is :%s\n", cpPassword);
	//		count = sk_PKCS12_SAFEBAG_num(bags);
	//		for (j = 0; j < count; j++)
	//		{
	//			bag = sk_PKCS12_SAFEBAG_value(bags, j);
	//			PKCS12_SAFEBAG_print(bp, bag);
	//		}
	//	}
	//}

	BIO_free(bp);
	//sk_PKCS7_pop_free(p7s, PKCS7_free);
	PKCS12_free(p12);
	return 0;
}

// sign with p12 private key
void generaCERFile(long lVersion, long lValidDays, int iSN, string strinCSRFile, string stroutCERFile, string strCACerFile, string strCaP12, string strPassword)
{

	X509 *pCACert = NULL;
	x509FromCertString(strCACerFile, &pCACert);
	if (pCACert==NULL)
	{
		return;
	}
	//EVP_PKEY *pKey = X509_get_pubkey(pCACert);

	EVP_PKEY *pKey = EVP_PKEY_new();
	getP12Key(strCaP12.c_str(), strPassword.c_str(), &pKey);

	X509_REQ *req = NULL;
	CSR2X509Req(&req, strinCSRFile.c_str());
	X509 *ptemp = X509_new();
	X509 *m_pClientCert;
	m_pClientCert = X509_new();

	X509_set_version(ptemp, lVersion);

	ASN1_INTEGER_set(X509_get_serialNumber(ptemp), iSN);

	X509_gmtime_adj(X509_get_notBefore(ptemp), 0);

	X509_gmtime_adj(X509_get_notAfter(ptemp), (long)60 * 60 * 24 * lValidDays);

	X509_set_subject_name(ptemp, X509_REQ_get_subject_name(req));

	X509_set_pubkey(ptemp, X509_PUBKEY_get(req->req_info->pubkey));

	X509_set_issuer_name(ptemp, X509_get_subject_name(pCACert));

	m_pClientCert = ptemp;
	X509V3_CTX ctx;
	X509V3_set_ctx(&ctx, pCACert, m_pClientCert, NULL, NULL, 0);

	X509_EXTENSION *x509_ext = X509_EXTENSION_new();
	string name_ext = "fan";
	string value_ext = "test";

	x509_ext = X509V3_EXT_conf(NULL, &ctx, (char*)name_ext.c_str(), (char*)value_ext.c_str());
	X509_add_ext(m_pClientCert, x509_ext, -1);

	// optimize later, should use CA private key, done
	X509_sign(m_pClientCert, pKey, EVP_md5());

	BIO *pbio = BIO_new_file(stroutCERFile.c_str(), "w");

	PEM_write_bio_X509(pbio, m_pClientCert);

	BIO_free(pbio);

	X509_REQ_free(req);

}
// sign with private key directly
void generaCERFile_1(long lVersion, long lValidDays, int iSN, string strinCSRFile, string strIssuerCer, string strSignKey, string strOutCERFile)
{

	X509 *pCACert = NULL;
	x509FromCertString(strIssuerCer, &pCACert);
	if (pCACert == NULL)
	{
		return;
	}

	EVP_PKEY *pSignKey = open_private_key(strSignKey.c_str(), 0);


	X509_REQ *req = NULL;
	CSR2X509Req(&req, strinCSRFile.c_str());
	X509 *ptemp = X509_new();
	X509 *pNewCert;
	pNewCert = X509_new();

	X509_set_version(ptemp, lVersion);

	ASN1_INTEGER_set(X509_get_serialNumber(ptemp), iSN);

	X509_gmtime_adj(X509_get_notBefore(ptemp), 0);

	X509_gmtime_adj(X509_get_notAfter(ptemp), (long)60 * 60 * 24 * lValidDays);

	X509_set_subject_name(ptemp, X509_REQ_get_subject_name(req));

	X509_set_pubkey(ptemp, X509_PUBKEY_get(req->req_info->pubkey));
	

	X509_set_issuer_name(ptemp, X509_get_subject_name(pCACert));

	pNewCert = ptemp;
	X509V3_CTX ctx;
	X509V3_set_ctx(&ctx, pCACert, pNewCert, NULL, NULL, 0);

	X509_EXTENSION *x509_ext = X509_EXTENSION_new();
	string name_ext = "fan";
	string value_ext = "test";

	x509_ext = X509V3_EXT_conf(NULL, &ctx, (char*)name_ext.c_str(), (char*)value_ext.c_str());
	X509_add_ext(pNewCert, x509_ext, -1);

	// sign with CA private key
	X509_sign(pNewCert, pSignKey, EVP_md5());

	BIO *pbio = BIO_new_file(strOutCERFile.c_str(), "w");

	PEM_write_bio_X509(pbio, pNewCert);

	BIO_free(pbio);

	X509_REQ_free(req);

}


void generaP12(string strCerFile, string strPrivateKey, string strOutp12)
{
	
	int ret, len, key_usage, iter, key_nid;
	PKCS12 *p12;
	PKCS7 *p7;
	STACK_OF(PKCS7) *safes;
	STACK_OF(PKCS12_SAFEBAG)*bags;
	PKCS12_SAFEBAG *bag;
	FILE  *fp;
	unsigned char *buf, *p, tmp[5000];
	X509  *cert = NULL;
	

	OpenSSL_add_all_algorithms();
	//der 
	//fp = fopen("certreq3.cer", "rb");
	//len = fread(tmp, 1, 5000, fp);
	//fclose(fp);
	//p = tmp;
	//d2i_X509(&cert, (const unsigned char**)&p, len);

	// pem
	BIO *bio = NULL;
	bio = BIO_new_file(strCerFile.c_str(), "r");
	if (bio == NULL)
	{
		return;
	}
	cert = PEM_read_bio_X509(bio, NULL, NULL, NULL);
	// private key 
	//fp = fopen(strPrivateKey.c_str(), "rb");
	//len = fread(tmp, 1, 5000, fp);
	//fclose(fp);
	//p = tmp;
	//EVP_PKEY *pkey = EVP_PKEY_new();
	//int iRet = i2d_PrivateKey(pkey, &p);

	EVP_PKEY *pkey = open_private_key(strPrivateKey.c_str(), 0);
	p12 = PKCS12_create("ossl", "friend name", pkey, cert, NULL, NID_pbe_WithSHA1And3_Key_TripleDES_CBC,

		NID_pbe_WithSHA1And40BitRC2_CBC, PKCS12_DEFAULT_ITER,

		-1, KEY_EX);

	len = i2d_PKCS12(p12, NULL);

	buf = p = (unsigned char *)malloc(len);

	len = i2d_PKCS12(p12, &p);

	fp = fopen(strOutp12.c_str(), "wb");

	fwrite(buf, 1, len, fp);

	fclose(fp);

	printf("ok\n");





	// method2 
	/*
	int                                              ret, len, key_usage, iter, key_nid;

	PKCS12                                     *p12;

	PKCS7                                       *p7;

	STACK_OF(PKCS7)                          *safes;

	STACK_OF(PKCS12_SAFEBAG)*bags;

	PKCS12_SAFEBAG                           *bag;

	FILE                                          *fp;

	unsigned char                      *buf, *p, tmp[5000];

	X509                                         *cert = NULL;

	EVP_PKEY                                 *pkey = NULL;



	OpenSSL_add_all_algorithms();

	p12 = PKCS12_init(NID_pkcs7_data);


	// original noted these codes
	//p12->mac=PKCS12_MAC_DATA_new();
	//p12->mac->dinfo->algor->algorithm=OBJ_nid2obj(NID_sha1);
	//ASN1_STRING_set(p12->mac->dinfo->digest,"aaa",3);
	//ASN1_STRING_set(p12->mac->salt,"test",4);
	//p12->mac->iter=ASN1_INTEGER_new();
	//ASN1_INTEGER_set(p12->mac->iter,3);
	// end



	// pkcs7 
	bags = sk_PKCS12_SAFEBAG_new_null();

	fp = fopen(strCerFile.c_str(), "rb");

	len = fread(tmp, 1, 5000, fp);

	fclose(fp);

	p = tmp;

	//cert 

	d2i_X509(&cert, (const unsigned char**)&p, len);

	bag = PKCS12_x5092certbag(cert);

	sk_PKCS12_SAFEBAG_push(bags, bag);

	/// private key 

	fp = fopen(strPrivateKey.c_str(), "rb");

	len = fread(tmp, 1, 5000, fp);

	fclose(fp);

	p = tmp;

	//pkey = i2d_PrivateKey(EVP_PKEY_RSA, NULL, (const unsigned char**)&p, len);
	int iRet = i2d_PrivateKey(pkey, &p);
	PKCS12_add_key(&bags, pkey, KEY_EX, PKCS12_DEFAULT_ITER, NID_pbe_WithSHA1And3_Key_TripleDES_CBC, "openssl");

	p7 = PKCS12_pack_p7data(bags);

	safes = sk_PKCS7_new_null();

	sk_PKCS7_push(safes, p7);

	ret = PKCS12_pack_authsafes(p12, safes);

	len = i2d_PKCS12(p12, NULL);

	buf = p = (unsigned char*)malloc(len);

	len = i2d_PKCS12(p12, &p);

	fp = fopen(strOutp12.c_str(), "wb");

	fwrite(buf, 1, len, fp);

	fclose(fp);

	printf("ok\n");
	*/
}

string Get_X509_ALGOR(BIO *bp, X509_ALGOR *signature)

{

	int nid;
	unsigned char*p;
	PBEPARAM *pbe = NULL;

	nid = OBJ_obj2nid(signature->algorithm);
	switch (nid)
	{
	case NID_md5WithRSAEncryption:
		return "md5WithRSAEncryption";
		break;

	case NID_sha1WithRSAEncryption:
		return "sha1WithRSAEncryption";
		break;

	case NID_rsaEncryption:
		return "rsaEncryption";
		break;

	case NID_sha1:
		return "sha1";
		break;

	case NID_pbe_WithSHA1And3_Key_TripleDES_CBC:
		return "NID_pbe_WithSHA1And3_Key_TripleDES_CBC";
		break;

	default:
		return "unknown signature.";
		break;

	}

	//optimize later, maybe should return the parameters
	//if (signature->parameter != NULL)

	//{
	//	if (nid == NID_pbe_WithSHA1And3_Key_TripleDES_CBC)
	//	{
	//		printf("算法参数:\n");
	//		p = signature->parameter->value.sequence->data;
	//		d2i_PBEPARAM(&pbe, (const unsigned char**)&p, signature->parameter->value.sequence->length);
	//		printf("salt : \n");
	//		i2a_ASN1_INTEGER(bp, pbe->salt);
	//		printf("\n");
	//		printf("iter : %d\n", ASN1_INTEGER_get(pbe->iter));
	//	}
	//}
	//printf("\n");
	//return 0;

}

void Get_X509_algor_digest(BIO *bp, X509_SIG *a, string &strAlgor, string &strDigest)

{

	if (a->algor != NULL)
	{
		printf("算法:\n");
		strAlgor = Get_X509_ALGOR(bp, a->algor);
	}
	if (a->digest != NULL)
	{
		printf("摘要:\n");
		i2a_ASN1_STRING(bp, a->digest, 1);
	}

}

void PKCS12_SAFEBAG_print(BIO *bp, PKCS12_SAFEBAG *bag)

{
	int nid, attrnum, certl, len = 50, k, n, x;
	unsigned char *p=NULL;
	char buf[50];
	PBEPARAM *pbe = NULL;
	X509_ATTRIBUTE *attr;
	ASN1_TYPE *type;
	X509 *cert = NULL;

	nid = OBJ_obj2nid(bag->type);
	if ((nid == NID_pkcs8ShroudedKeyBag) || (nid == NID_pbe_WithSHA1And3_Key_TripleDES_CBC))  /* pkcs 8 */
	{
		nid = OBJ_obj2nid(bag->value.shkeybag->algor->algorithm);
		if (nid == NID_pbe_WithSHA1And3_Key_TripleDES_CBC)
		{
			/* alg */
			//X509_SIG_print(bp, bag->value.shkeybag);
		}

	}
	else if (nid == NID_certBag)
	{
		nid = OBJ_obj2nid(bag->value.bag->type);
		if (nid == NID_x509Certificate)
		{
			p = bag->value.bag->value.x509cert->data;
			certl = bag->value.bag->value.x509cert->length;
			d2i_X509(&cert, (const unsigned char**)&p, certl);
			if (cert != NULL)
			{
				X509_print(bp, cert);
			}
		}
	}
	printf("attris : \n");
	attrnum = sk_X509_ATTRIBUTE_num(bag->attrib);
	for (k = 0; k < attrnum; k++)
	{
		attr = sk_X509_ATTRIBUTE_value(bag->attrib, k);
		nid = OBJ_obj2nid(attr->object);
		OBJ_obj2txt(buf, len, attr->object, 1);
		printf("object : %s,nid is %d\n", buf, nid);
		if (attr->single == 0) /* set */
		{
			n = sk_ASN1_TYPE_num(attr->value.set);
			for (x = 0; x < n; x++)
			{
				type = sk_ASN1_TYPE_value(attr->value.set, x);
				if ((type->type != V_ASN1_SEQUENCE) && (type->type != V_ASN1_SET))
				{
					if (type->type == V_ASN1_OCTET_STRING)
						i2a_ASN1_INTEGER(bp, type->value.octet_string);
					else
						ASN1_STRING_print(bp, (ASN1_STRING *)type->value.ptr);
				}
			}
		}
		printf("\n");
	}

}



// calculate 24
double add(double a, double b);
double subtract(double a, double b);
double multiplicat(double a, double b);
double divide(double a, double b);
char ch[4][20] = { "+", "-", "*", "/" };
double(*func[])(double a, double b) = { add, subtract, multiplicat, divide };


double add(double a, double b)
{
	return a + b;
}

double subtract(double a, double b)
{
	return a - b;
}

double multiplicat(double a, double b)
{
	return a * b;
}

double divide(double a, double b)
{
	return a / b;
}

int calc24(int iNum0, int iNum1, int iNum2, int iNum3)
{
	int dArr[4] = {0,0,0,0};

	dArr[0] = iNum0;
	dArr[1] = iNum1;
	dArr[2] = iNum2;
	dArr[3] = iNum3;


	string strResult = "";
	double fRet = 24.0;
	for (int iArr1 = 0; iArr1 < 4; iArr1++)
	for (int iArr2 = 0; iArr2 < 4; iArr2++)
	for (int iArr3 = 0; iArr3 < 4; iArr3++)
	for (int iArr4 = 0; iArr4 < 4; iArr4++)    // 四个数字出现的情况
	if (iArr2 != iArr1 && iArr1 != iArr3 && iArr1 != iArr4 && iArr2 != iArr3 && iArr2 != iArr4 && iArr3 != iArr4)
	{
		for (int j = 0; j < 4; j++)
		for (int k = 0; k < 4; k++)
		for (int m = 0; m < 4; m++)            // 每两个数字间出现的符号
		if (func[m](func[k](func[j](dArr[iArr1], dArr[iArr2]), dArr[iArr3]), dArr[iArr4]) == fRet)
		{
			// 123  （括号指定符号的计算顺序）
			//strResult = to_string(dArr[iArr1]) + to_string(ch[j]) +
			
			//cout << "((" << dArr[iArr1] << ch[j] << dArr[iArr2] << ")" << ch[k] << dArr[iArr3] << ")" << ch[m] << dArr[iArr4] << " = 24" << endl;
			return 0;
		}
		else if (func[m](func[j](dArr[iArr1], dArr[iArr2]), func[k](dArr[iArr3], dArr[iArr4])) == fRet)
		{
			//132  （括号指定符号的计算顺序）
			//cout << "(" << dArr[iArr1] << ch[j] << dArr[iArr2] << ")" << ch[m] << "(" << dArr[iArr3] << ch[k] << dArr[iArr4] << ") = 24" << endl;
			return 0;
		}
		else if (func[m](func[k](dArr[iArr1], func[j](dArr[iArr2], dArr[iArr3])), dArr[iArr4]) == fRet)
		{
			//213  （括号指定符号的计算顺序）
			//cout << "(" << dArr[iArr1] << ch[k] << "(" << dArr[iArr2] << ch[j] << dArr[iArr3] << "))" << ch[m] << dArr[iArr4] << " = 24" << endl;
			return 0;
		}
		else if (func[m](dArr[iArr1], func[k](func[j](dArr[iArr2], dArr[iArr3]), dArr[iArr4])) == fRet)
		{
			//231  （括号指定符号的计算顺序）
			//cout << dArr[iArr1] << ch[m] << "((" << dArr[iArr2] << ch[j] << dArr[iArr3] << ")" << ch[k] << dArr[iArr4] << ")" << " = 24" << endl;
			return 0;
		}
		else if (func[m](dArr[iArr1], func[k](dArr[iArr2], func[j](dArr[iArr3], dArr[iArr4]))) == fRet)
		{
			//321  （括号指定符号的计算顺序）
			//cout << dArr[iArr1] << ch[m] << "(" << dArr[iArr2] << ch[k] << "(" << dArr[iArr3] << ch[j] << dArr[iArr4] << "))" << " = 24" << endl;
			return 0;
		}
	}

	//cout << "cannot calculate 24 " << endl;
	return -1;
}





