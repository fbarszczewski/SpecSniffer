# Overall Description
Program created for needs in my workplace in refubrish process. Its main purpose is to check hardware specification of Windows based devices, basic diagnostics, installing drivers and saving hardware info of device in database.

## Hardware Specification
All hardware data is gained by querying through Windows Management Instrumentation (WMI) classes. In some cases like when getting hard drive S.M.A.R.T data Windows require administrator privileges. That's why by default this program is runned 'as admin...'.

## Diagnostics
By using this app You can perform some basic diagnostics of device.
#### S.M.A.R.T Check
If hard drive connected to device have rellocated sectors program will inform user by displaying explamation mark near coruppted hdd.
#### Screen Test
User can check for any screen spots or dead pixels by perform basic screen test. It will display in full screen plain colors that helps user indicating screen defects like spots or dead pixels.
#### Camera & Audio Test
User can check camera and microphone input and perform speakers test.
#### Battery status
Displaying battery health charging rate and power left helps user indicating if battery is defected.
#### Device Drivers
Presenting device drivers with different status than good.

## Drivers Installation
Drivers installation can be done by two ways. By using Windows update or local drivers database. Local server have driver packs prepared for each model. This program will connect to this server and install drivers by running batch file.

## Saving data
All data of device that running this program can be saved in local MySql database located in my workplace. This data will be used in further refurbishment process.
