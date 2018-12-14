
copy TripleDes.h "../../TripleDesExe\TripleDesExe" /Y
cd ../
cd Debug
copy TripleDesDll.dll "../../TripleDesExe\Debug" /Y
copy TripleDesDll.dll "../../BitcoinTerminal\Libraries" /Y
copy TripleDesDll.lib "../../TripleDesExe\lib" /Y

pause
