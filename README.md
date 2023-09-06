# Framework
Framework is an older project originally written in .NET Framework 4.0. It served as a crucial toolkit during the pre-.NET Core era, offering custom versions of various commonly used tools that I employed in-house when mainstream alternatives weren't suitable. Though I've since moved on to newer technologies, I believe this project stands as a testament to my coding skills.

## Framework.Common

**Framework.Common** houses my custom implementations of various common classes, including:

* LogWriter: A versatile logging utility capable of logging to the console, file, WMI events, and the Windows Event Logger.
* RegEx: Utilized for email validation and password strength checking.

## Framework.DataAccess

**Framework.DataAccess** is my custom SQL Object-Relational Mapping (ORM) implementation. It came into play during the days when older versions of Entity Framework fell short in comparison to the newer EF .CORE versions.

## Framework.HL7

**Framework.HL7** is a compact utility I developed to assist in parsing HL7 messages. This tool was invaluable during my efforts to learn and work with the HL7 standard.

## Framework.Web

**Framework.Web** comprises a helper class designed to streamline the execution of GET and POST requests from the server-side.
## Framework.UnitTests

**Framework.UnitTests** houses the unit tests for the entire project, ensuring the reliability and correctness of the components.
