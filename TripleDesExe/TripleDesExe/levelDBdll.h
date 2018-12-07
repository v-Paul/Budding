#pragma once

#ifdef DLL_EXPORTS  
#define DLL_API __declspec(dllexport)   
#else  
#define DLL_API __declspec(dllimport)   
#endif 

using namespace std;

extern "C" DLL_API char* OpenDB(const char* cDBPath);
extern "C" DLL_API void CloseDB();
extern "C" DLL_API char* GetValue(const char* Key);
extern "C" DLL_API char* PutKeyValue(const char* Key, const char* Value);
extern "C" DLL_API char* DelItm(const char* Key);
extern "C" DLL_API char* AddDbOps2Batch(char* strFun, char* strKey, char* strValue);
extern "C" DLL_API char* WriteBatch();

extern "C" DLL_API char* GetfirstKey();
extern "C" DLL_API char* GetlastKey();
extern "C" DLL_API char* GetNextKey();
extern "C" DLL_API char* GetKey(int iPos);

