#include<string>
//#include <cassert>
#include <list>
#include "levelDBdll.h"
#include "leveldb/db.h"
#include "leveldb/write_batch.h"

leveldb::DB* db= nullptr;
leveldb::Iterator* it = nullptr; 
enum emDBOs
{
	ePutKeyValue,
	eDelItm
};
struct DBOperator
{
	emDBOs emfun;
	string key;
	string value;
};

list<DBOperator> lstDBOperators;



//base operate faction
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

//

char* OpenDB(const char* cDBPath)
{
	//string OK = "OK";
	//if (db!=nullptr)
	//{
	//	return ConstChar2Char(OK.c_str());
	//}
	//leveldb::DB* db;
	leveldb::Options options;
	options.create_if_missing = true;
	leveldb::Status s = leveldb::DB::Open(options, cDBPath, &db);
	if (s.ok())
	{
		it = db->NewIterator(leveldb::ReadOptions());
	}


	string strRet = s.ToString();
	return ConstChar2Char(strRet.c_str());

}

void CloseDB()
{
	if (it != nullptr)
	{
		delete it;
	}
	if (db!= nullptr)
	{
		delete db;
	}

	
}
char* GetValue(const char* Key)
{
	std::string strValue;
	leveldb::Status s = db->Get(leveldb::ReadOptions(), Key, &strValue);
	if (s.ok())
	{
		return ConstChar2Char(strValue.c_str() );
	}
	else
	{
		return  nullptr;
	}

}

char* PutKeyValue(const char* Key, const char* Value)
{
	leveldb::WriteOptions write_options;
	write_options.sync = true;
	leveldb::Status s = db->Put(write_options, Key, Value);;
	string strRet = s.ToString();
	return ConstChar2Char(strRet.c_str());

}

char* DelItm(const char* Key)
{

	leveldb::Status s = db->Delete(leveldb::WriteOptions(), Key);
	string strRet = s.ToString();
	return ConstChar2Char(strRet.c_str());

}

char* AddDbOps2Batch(char* strFun, char* strKey, char* strValue)
{
	emDBOs emtemp;
	if (strFun == "PutKeyValue")
		emtemp = emDBOs::ePutKeyValue;
	else if (strFun == "DelItm")
		emtemp = emDBOs::eDelItm;
	else
		return ConstChar2Char("NotSupportOper");

	DBOperator dbo ;
	dbo.emfun = emtemp;
	dbo.key = strKey;
	dbo.value = strValue;
	lstDBOperators.push_back(dbo);

	return ConstChar2Char("ok");


}
char* WriteBatch()
{
	if (lstDBOperators.size() == 0)
	{
		return ConstChar2Char("EmptyBatch");
	}

	leveldb::WriteBatch batch;
	
	for each (DBOperator dbo in lstDBOperators)
	{
		switch (dbo.emfun)
		{
		case emDBOs::ePutKeyValue:
			batch.Put(dbo.key, dbo.value);
			break;
		case emDBOs::eDelItm:
			batch.Delete(dbo.key);
			break;
			
		}
	}

	leveldb::Status s = db->Write(leveldb::WriteOptions(), &batch);

	lstDBOperators.clear();
	string strRet = s.ToString();
	return ConstChar2Char(strRet.c_str());
		
}

//char* GetKey(const char* startPos, int idirect)


char* GetfirstKey()
{
	if (it == nullptr)
	{
		return nullptr;
	}
	it->SeekToFirst();

	if (!it->Valid())
	{
		return nullptr;

	}
	string sKey = it->key().ToString();
	return ConstChar2Char(sKey.c_str());
}

char* GetlastKey()
{
	if (it == nullptr)
	{
		return nullptr;
	}
	it->SeekToLast();

	if (!it->Valid())
	{
		return nullptr;

	}
	string sKey = it->key().ToString();
	return ConstChar2Char(sKey.c_str());
}

char* GetNextKey()
{
	if (it == nullptr)
	{
		return nullptr;
	}
	it->Next();

	if (!it->Valid())
	{
		return nullptr;

	}
	string sKey = it->key().ToString();
	return ConstChar2Char(sKey.c_str());
}


char* GetKey(int iPos)
{
	if (it==nullptr)
	{
		return nullptr;
	}

	it->SeekToFirst();
	for (int i = 0; i < iPos; i++)
	{
		it->Next();
		if (!it->Valid())
		{
			return nullptr;
		}
	}

	if (!it->Valid())
	{
		return nullptr;
	}
	string sKey = it->key().ToString();
	return ConstChar2Char(sKey.c_str());
		
}
