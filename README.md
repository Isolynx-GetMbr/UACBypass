# UACBypass (Registry Hijacking)

This uses both fodhelper and eventwvr method to bypass UAC varying on the windows versions. 

### Forums
- [EventWvr method (< Win10 2016 builds)](https://www.fortinet.com/blog/threat-research/offense-and-defense-a-tale-of-two-sides-bypass-uac)
- [Fodhelper method (> Win10)](https://www.linkedin.com/pulse/uac-bypass-using-fodhelperexe-shivam-gupta-ra0xc)

>[!TIPS]
> Wow64DisableWow64FsRedirection and Wow64RevertWow64FsRedirection is used to prevent redirection to SysWOW64 directory when trying to access System32 especially if your process is 32-bit. Since both fodhelper and eventvwr doesn't exists on SysWOW64 somehow.

>[!CAUTION]
> This is published **ONLY** for cybersecurity defense purposes. Please don't use it in such illegal ways!


