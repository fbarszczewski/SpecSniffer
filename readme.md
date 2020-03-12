# Overall Description
Program created for needs in my workplace. Its main purpose is to check hardware specification of Windows based devices, basic diagnostics, installing drivers and saving hardware info of device in database.

## Hardware Specification
All hardware data is gained by querying through Windows Management Instrumentation (WMI) classes. In some cases like when getting hard drive S.M.A.R.T data Windows require administrator privileges. That's why by default this program is runned 'as admin...'.
