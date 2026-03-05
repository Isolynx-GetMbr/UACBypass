# UACBypass (Registry Hijacking)

This uses both fodhelper and eventvwr method to bypass UAC varying on the windows versions, since both of them are one of the trusted binaries
of windows which can be abused by malwares to execute the executable which is the malware itself without the UAC prompt.

### Forums
- [Eventvwr method (< Win10 2016 builds)](https://www.fortinet.com/blog/threat-research/offense-and-defense-a-tale-of-two-sides-bypass-uac)
- [Fodhelper method (> Win10)](https://www.linkedin.com/pulse/uac-bypass-using-fodhelperexe-shivam-gupta-ra0xc)

>[!NOTE]
> There are some issues going by, especially on older versions (< win 10) since I've only tested this on the other computer and my main broke 2 years ago so don't forget to remind me by sending an issue after you tested it. 

>[!TIP]
> **Wow64DisableWow64FsRedirection** and **Wow64RevertWow64FsRedirection** is used to prevent redirection to **SysWOW64** directory when trying to access **System32** especially if your process is *32-bit*. Since both *fodhelper* and *eventvwr* doesn't exists on **SysWOW64** somehow.

>[!CAUTION]
> This is published **ONLY** for cybersecurity defense purposes. Please **don't** use it in such illegal ways!


