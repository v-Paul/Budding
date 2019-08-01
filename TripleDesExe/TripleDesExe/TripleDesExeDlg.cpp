
// TripleDesExeDlg.cpp : ʵ���ļ�
//

#include "stdafx.h"
#include "TripleDesExe.h"
#include "TripleDesExeDlg.h"
#include "afxdialogex.h"

#include "TripleDes.h"
#include <iostream>
#include "levelDBdll.h"
#include <list>

extern "C"{
#include "applink.c"
};

using namespace std;
#include <atlstr.h>


#pragma comment(lib,"TripleDesDll.lib")
#pragma comment(lib,"LevelDBdll.lib")
#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// ����Ӧ�ó��򡰹��ڡ��˵���� CAboutDlg �Ի���

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// �Ի�������
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

// ʵ��
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CTripleDesExeDlg �Ի���



CTripleDesExeDlg::CTripleDesExeDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CTripleDesExeDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CTripleDesExeDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT1, m_edSrc);
	DDX_Control(pDX, IDC_EDIT2, m_edEnc);
	DDX_Control(pDX, IDC_BUTTON3, m_btnTest);
	DDX_Control(pDX, IDC_COMBO1, m_cbbDes);
}

BEGIN_MESSAGE_MAP(CTripleDesExeDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON1, &CTripleDesExeDlg::OnBnClickedButton1)
	ON_BN_CLICKED(IDC_BUTTON2, &CTripleDesExeDlg::OnBnClickedButton2)
	ON_BN_CLICKED(IDC_BUTTON3, &CTripleDesExeDlg::OnBnClickedButton3)
	ON_BN_CLICKED(IDOK, &CTripleDesExeDlg::OnBnClickedOk)
	ON_BN_CLICKED(IDCANCEL, &CTripleDesExeDlg::OnBnClickedCancel)
	ON_BN_CLICKED(IDC_BUTTON4, &CTripleDesExeDlg::OnBnClickedButton4)
END_MESSAGE_MAP()


// CTripleDesExeDlg ��Ϣ�������

BOOL CTripleDesExeDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// ��������...���˵�����ӵ�ϵͳ�˵��С�

	// IDM_ABOUTBOX ������ϵͳ���Χ�ڡ�
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// ���ô˶Ի����ͼ�ꡣ  ��Ӧ�ó��������ڲ��ǶԻ���ʱ����ܽ��Զ�
	//  ִ�д˲���
	SetIcon(m_hIcon, TRUE);			// ���ô�ͼ��
	SetIcon(m_hIcon, FALSE);		// ����Сͼ��

	// TODO:  �ڴ���Ӷ���ĳ�ʼ������

	//m_btnTest.ShowWindow(SW_HIDE);
	m_cbbDes.InsertString(0, _T("3Des_ECB"));
	m_cbbDes.InsertString(1, _T("3Des_CBC"));
	m_cbbDes.SetCurSel(0);

	GetDlgItem(IDOK)->ShowWindow(SW_HIDE);
	GetDlgItem(IDCANCEL)->ShowWindow(SW_HIDE);

	return TRUE;  // ���ǽ��������õ��ؼ������򷵻� TRUE
}

void CTripleDesExeDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// �����Ի��������С����ť������Ҫ����Ĵ���
//  �����Ƹ�ͼ�ꡣ  ����ʹ���ĵ�/��ͼģ�͵� MFC Ӧ�ó���
//  �⽫�ɿ���Զ���ɡ�

void CTripleDesExeDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // ���ڻ��Ƶ��豸������

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// ʹͼ���ڹ����������о���
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// ����ͼ��
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

//���û��϶���С������ʱϵͳ���ô˺���ȡ�ù��
//��ʾ��
HCURSOR CTripleDesExeDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



void CTripleDesExeDlg::OnBnClickedButton1()
{
	// TODO:  �ڴ���ӿؼ�֪ͨ����������
	
	
	int nSel = m_cbbDes.GetCurSel();
	
	if (0 == nSel)          // ECB
	{
		CString str, strEnd;
		m_edSrc.GetWindowText(str);
		char* ch = str.GetBuffer(str.GetLength());
		char* pEnd = TRIPLE_ECB_EncryptDefaultKey(ch);
		strEnd.Format(_T("%s"), pEnd);
		m_edEnc.SetWindowText(strEnd);
		str.ReleaseBuffer();
	}
	else if (1 == nSel)    // CBC
	{
		CString str, strEnd;
		m_edSrc.GetWindowText(str);
		char* ch = str.GetBuffer(str.GetLength());
		char* pEnd = TRIPLE_CBC_EncryptDefaultKeyIV(ch);
		strEnd.Format(_T("%s"), pEnd);
		m_edEnc.SetWindowText(strEnd);
		str.ReleaseBuffer();
	}
}


void CTripleDesExeDlg::OnBnClickedButton2()
{
	// TODO:  �ڴ���ӿؼ�֪ͨ����������
	
	
	
	int nSel = m_cbbDes.GetCurSel();
	if (0 == nSel)                        // ECB
	{
		CString str, strEnd;
		m_edEnc.GetWindowText(str);
		char* ch = str.GetBuffer(str.GetLength());
		char* pEnd = TRIPLE_ECB_DecryptDefaultKey(ch);
		strEnd.Format(_T("%s"), pEnd);
		m_edSrc.SetWindowText(strEnd);
		str.ReleaseBuffer();
	}
	else if (1 == nSel)                  // CBC
	{
		CString str, strEnd;
		m_edEnc.GetWindowText(str);
		char* ch = str.GetBuffer(str.GetLength());
		char* pEnd = TRIPLE_CBC_DecryptDefaultKeyIV(ch);
		strEnd.Format(_T("%s"), pEnd);
		m_edSrc.SetWindowText(strEnd);
		str.ReleaseBuffer();
	}

}


void CTripleDesExeDlg::OnBnClickedButton3()
{
	//int ret = -1;
	string strTest = "123456";
	//string strHash = sha256((char*)strTest.c_str(), strTest.length());
	string strEndata = "";
	string strPubKey = "C:\\Users\\phony\\Desktop\\temp\\pubkey.pem";
	string strPriKey = "C:\\Users\\phony\\Desktop\\temp\\prikey.pkcs8";
	strEndata = rsa_pub_encrypt(strTest.c_str(), strTest.length(), strPubKey.c_str(), true);

	string strOrigina = rsa_pri_decrypt(strEndata.c_str(), strEndata.length(), strPriKey.c_str(), true);
	
	strOrigina = "";
	strEndata = rsa_pri_sign(strTest.c_str(), strTest.length(), strPriKey.c_str(), true);
	strOrigina = rsa_pub_DecryptSignature(strEndata.c_str(), strEndata.length(), strPubKey.c_str(), true);
	//
	//string strSignature = rsa_pri_sign(strTest.c_str(), strTest.length(), strPriKey.c_str(), true);
	//string stroldData = rsa_pub_DecryptSignature(strSignature.c_str(), strSignature.length(), strPubKey.c_str(), true);
	//string strRet = rsa_pri_sign_byKeyPath(strTest.c_str(), "C:\\VTM\\BitcoinTerminal\\prikey.pem");
	//string strOrigina = rsa_pub_checkSignature_byKeyPath(strRet.c_str(), "C:\\VTM\\BitcoinTerminal\\pubkey.pem");
	//generateRSAKey2File("C:\\VTM\\BitcoinTerminal\\PubKey.pem","C:\\VTM\\BitcoinTerminal\\PriKey.pem");
	//generaCSRFile("C:\\VTM\\XFSTool\\TripleDesExe\\1106_2.csr", 1024, 1, 0x10001L, "Mytest_csr", "CN", "SX", "Xi'an","CSI","VTM PRO");
	

	//generaCERFile(1, 365, 3, "C:\\VTM\\XFSTool\\TripleDesExe\\1106_2.csr",
	//	"C:\\VTM\\XFSTool\\TripleDesExe\\1106_2_1.cer",
	//	"C:\\VTM\\XFSTool\\TripleDesExe\\root-cert.cer",
	//	"C:\\VTM\\XFSTool\\TripleDesExe\\root.p12",
	//	"012345");

	//generaCERFile_1(1, 365, 0, "C:\\VTM\\XFSTool\\TripleDesExe\\1106_2.csr",
	//	"C:\\VTM\\XFSTool\\TripleDesExe\\yang_ca.cer",
	//	"C:\\VTM\\XFSTool\\TripleDesExe\\yang.key.pem",
	//	"C:\\VTM\\XFSTool\\TripleDesExe\\1106_2.cer" );

	//generaP12( "C:\\VTM\\XFSTool\\TripleDesExe\\1106_2.cer", 
	//		   "C:\\VTM\\XFSTool\\TripleDesExe\\1106_2.key",
	//		   "C:\\VTM\\XFSTool\\TripleDesExe\\1106_2.p12");

	//int Nums[] = {1,2,3,4};
	//bool bOK = calc24(Nums);

	//int Nums1[] = {1,1,1,1};
	//bool bOK1 = calc24(Nums1);

	//string sRet = OpenDB("C:\\Users\\phony\\Documents\\XXPClient\\test");

	//string sRet1 = OpenDB("C:\\Users\\phony\\Documents\\XXPClient\\leveldb222");
	////string sputRet = PutKeyValue("testKey", "testValue");
	//string lastKey = GetlastKey();
	//string preKey = "10566987FAD49F8B64F7B4AB05D2A62919B7D8380D43935F9D9B5385B09CD4AC";
	//string lastValue = GetValue(preKey.c_str());
	//string strRet = PutKeyValue("LastKey", lastValue.c_str());


	//sRet = PutKeyValue("key0", "value0");
	//sRet = PutKeyValue("key1", "value1");
	//sRet = PutKeyValue("key2", "value2");
	//sRet = PutKeyValue("key3", "value3");

	//list<string> strKes;
	//string lastKey = GetKey(-1, 0);
	//strKes.push_back(lastKey);

	//sRet = PutKeyValue("key5", "value5");
	//CloseDB();

	//string sRet = OpenDB("C:\\Users\\phony\\Documents\\XXPClient\\test");

	//string lastKey = GetlastKey();

	//string lastKey1 = GetKey(-1, 0);
	//strKes.push_back(lastKey);

	//string skey0 = GetKey(0, 0);
	//strKes.push_back(skey0);

	//while (true)
	//{
	//	char* cpKey = GetKey(-2, 1);
	//	if (cpKey != nullptr)
	//	{
	//		string temp = string(cpKey);
	//		strKes.push_back(temp);
	//	}
	//	else
	//	{
	//		break;
	//	}
	//}


	//list<string>::iterator itor;
	//itor = strKes.begin();
	//while (itor != strKes.end())
	//{
	//	string str = *itor;
	//	itor++;
	//	string strRet = DelItm(str.c_str());
	//}
	

	 CloseDB();

}


void CTripleDesExeDlg::OnBnClickedOk()
{
	// TODO:  �ڴ���ӿؼ�֪ͨ����������
	// CDialogEx::OnOK();
}


void CTripleDesExeDlg::OnBnClickedCancel()
{
	// TODO:  �ڴ���ӿؼ�֪ͨ����������
	CDialogEx::OnCancel();
}


//PKCS8
void CTripleDesExeDlg::OnBnClickedButton4()
{
	// TODO:  �ڴ���ӿؼ�֪ͨ����������

	string strEndata = "";

	string strPubKey = "C:\\Users\\phony\\Desktop\\temp\\pubkey.pem";
	string strPriKey = "C:\\Users\\phony\\Desktop\\temp\\prikey.pkcs8";
	
	generateRSAPkcs8Key2File(strPubKey.c_str(), strPriKey.c_str());




}
