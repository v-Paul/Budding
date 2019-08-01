#include <string>
using namespace std;
#define DLL_EXPORTS
#ifdef DLL_EXPORTS  
#define DLL_API __declspec(dllexport)   
#else  
#define DLL_API __declspec(dllimport)   
#endif  
#define PUB_KEY_FILE "pubkey.pem"    // ¹«Ô¿Â·¾¶  
#define PRI_KEY_FILE "prikey.pem"    // Ë½Ô¿Â·¾¶  

extern "C" DLL_API  char* TRIPLE_ECB_Encrypt(char* source, char* key);

extern "C" DLL_API  char* TRIPLE_ECB_Decrypt(char* source, char* key);

extern "C" DLL_API  char* TRIPLE_CBC_Encrypt(char* source, char* key, char* cbc_IV);

extern "C" DLL_API  char* TRIPLE_CBC_Decrypt(char* source, char* key, char* cbc_IV);


extern "C" DLL_API  char* TRIPLE_ECB_EncryptDefaultKey(char* source);

extern "C" DLL_API  char* TRIPLE_ECB_DecryptDefaultKey(char* source);

extern "C" DLL_API  char* TRIPLE_CBC_EncryptDefaultKeyIV(char* source);

extern "C" DLL_API  char* TRIPLE_CBC_DecryptDefaultKeyIV(char* source);


extern "C" DLL_API  void generateRSAKey(string strKey[2]);

extern "C" DLL_API  void md5(const std::string &srcStr, std::string &encodedStr, std::string &encodedHexStr);

//extern "C" DLL_API  void sha256(const std::string &srcStr, std::string &encodedStr, std::string &encodedHexStr);

extern "C" DLL_API  char* sha256(char* pcOrigana, int iLength);



extern "C" DLL_API  char* rsa_pub_encrypt(const char* clearText, int iTxtlen, const char* pubKey, bool bIsKeybyPath);

extern "C" DLL_API  char* rsa_pri_decrypt(const char* cipherText, int iTxtlen, const char* priKey, bool bIsKeybyPath);



extern "C" DLL_API  char* rsa_pri_sign(const char* pcText, int iTxtlen, const char* pcPriKey, bool bIsKeybyPath);

extern "C" DLL_API  char* rsa_pub_DecryptSignature(const char* pcText, int iTxtlen, const char* pubKey, bool bIsKeybyPath);


//extern "C" DLL_API void generaCSR();
extern "C" DLL_API void generate_Key();

extern "C" DLL_API void generateRSAKey2File(const char* pcPubKeyPath = PUB_KEY_FILE, const char* pcPriKeyPath = PRI_KEY_FILE);
extern "C" DLL_API void generateRSAPkcs8Key2File(const char* pcPubKeyPath = PUB_KEY_FILE, const char* pcPriKeyPath = PRI_KEY_FILE);
extern "C" DLL_API int generaCSR_1();
//extern "C" DLL_API int CSR2X509(X509_REQ **req);
extern "C" DLL_API void x509FromCertString(string cert);
extern "C" DLL_API void generaCER();
extern "C" DLL_API void generaP12(string strCerFile, string strPrivateKey, string strOutp12);


extern "C" DLL_API int generaCSRFile(string strCSRFolder = "", int iRSALen = 1024, int iVersion = 1, unsigned long ulMod = 0x10001L, string strcommonName = "opsss_csr",
	string strcountryName = "CN", string strproviceName = "SX", string strcityName = "XA", string strorganizationName = "CSI",
	string strorganizationalUnitName = "VTM DEV", bool bIsOutPem = true);

extern "C" DLL_API void generaCERFile(long lVersion, long lValidDays, int iSN, string strinCSRFile, string stroutCERFile, string strCACerFile, string strCaP12, string strPassword);

extern "C" DLL_API void generaCERFile_1(long lVersion, long lValidDays, int iSN, string strinCSRFile, string strIssuerCer, string strSignKey, string strOutCERFile);

extern "C" DLL_API int calc24(int iNum0, int iNum1, int iNum2, int iNum3);


