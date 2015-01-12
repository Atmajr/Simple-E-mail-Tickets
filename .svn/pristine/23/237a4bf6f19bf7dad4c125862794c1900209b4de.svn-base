QP emailTicket 1.0
author: Justin R. Acker
company: PracticePro Software Systems
purpose: allow end users to easily create e-mail delivered trouble tickets for issues with other software

Context:
--------
1.) Usage and Code Explanation
2.) Using the confguration files
	2a.)config.txt
	2b.)emailList.txt
	2c.)ticketTemplate.txt
	2d.)autoResponse.txt
3.) Contact information
4.) Disclaimer

1.) Usage and Code Explanation:
----

emailTicket is designed to be a low-power, low-requirement solution for trouble ticketing in an environment which require minimal oversight and does not have a need for end-user accounts and ticket updates.
When a client visits Index.aspx, they are presented with a form into which they can enter their contact information and the nature of their issue.
On PageLoad, Index.aspx looks for a QueryString in format ?CIN=99999&RegisteredName=John%20Doe&ProductType=Professional&ProductVersion=6.1.2&OSVersion=6.1.7601.2
If it finds the variable CIN in the QueryString, it will auto-populate the CIN textbox with that value.

Each text field on the form has a number of validations associated with it.
-Name:			on submit - validates name is present
-CIN:			on text changed - validates cin is five numeric digits; on submit - validate cin is present
-E-mail:		on text changed - validate e-mail looks valid; on submit - validates either e-mail or phone is present
-Phone:			on text changed - validate phone appeares to be a valid US phone number; on submit - validates either e-mail or phone is present
-ContactPref:	on submit - if a contact prefererence has been selected, verified the related field is populated (i.e. if you choose phone, you must include a phone number)
-Issue:			on submit - validates issue is present

On submit, if all fields validate, a trouble ticket e-mail is generated to the support e-mail that is next up in rotation. 
The rotation is determined by a variable, emailCount, stored in config.txt

Once the e-mail has been generated, the user is redirected to Complete.aspx which presents a small message confirming that the ticket has been generated and,
if autoRespond is turned on and they entered and e-mail address, that a receipt has been sent to them. This page can be edited to include any information you wish.
Currently, it simply presents a link back to the QuickPractice home page.

2.)Using the configuration files
----

emailTicket comes packaged with four plaintext configuration files which can be edited to allow for some control over the app without the need to recompile.
The usage and formatting of these files is explained below.

2a.) config.txt
----

config.txt allows you to set the four global options for the app.

	-emailCount		- this is the incremental counter which determines which support engineer will receive the next ticket. DO NOT TOUCH THIS unless you are resetting it to 0.
	-ccToManager	- this variable determines whether trouble tickets should be CC: to -ALL- e-mails designated "manager" in emailList.txt. 1 = true, 0 = false. Default value: 1.
	-ccToGeneral	- this variable determines whether trouble tickets should be BCC: to -ALL- e-mails designated "general" in emailList.txt. 1 = true, 0 = false. Default value: 0.
	-autoRespond	- this variable determines whether auto-response receipts should be e-mailed to the end-user. Whether this is turned on or not, an auto-response will only be generated if the end-use supplies her e-mail address. 1 = true, 0 = false. Default value: 1.
	-ticketCounter	- represents the number of tickets sent since the variable was last manually reset

2b.) emailList.txt
----

emailList.txt holds the list of e-mail addresses for the staff behind the software. Any number of e-mail addresses can be added.
format:
[e-mail type]|[e-mail address]
i.e.
manager|manager@company.com

legal e-mail types:
manager
general
support

2c.) ticketTemplate.txt
----

ticketTemplate.txt maintains the template for the body of the e-mail that is sent to the support employee. 
You can edit this as you like, and variables within it will be automatically replaced at run-time - you may re-use, or not use, variables as you see fit.
The formatting preserves line-breaks, but does not maintain other formatting.

Variables:

<datetime>		- replaced with the current server date and time in MMM ddd DD HH:MM YYYY format
<name>			- name verbatim from textbox
<cin>			- cin verbatim from textbox - gleaned from QueryString as ?CIN=
<email>			- email verbatim from textbox
<phone>			- phone verbatim from textbox
<contactPref>	- based on state of Contact Preference radio button at submit time - "E-mail", "Phone", or "No Preference"
<issue>			- issue verbatim from textbox; formatting not preserved 

The below variables are only sent with the ticket if they were present in the QueryString. There are no form elements related to them.

QueryString variables:

<method>		- the method through which the customer reached the webform. Returns "Web Site" if no QueryString is present, or "QuickPractice Software" if a QueryString is detected
<registeredName>	- the name registered in the QuickPractice software - QueryString as ?RegisteredName=
<productType>		- type of QuickPractice product (Standard, Deluxe, Professional) - QueryString as ?ProductType=
<productVersion>	- version number of QuickPractice product - QueryString as ?ProductVersion=	
<osVersion>			- version of the Windows OS. Sent as a build number in the QueryString as ?OSVersion= . If the build is recognized, emailTicket sends a human readable version number (e.g. Windows 7), otherwise it sends a build number (e.g. Windows Build 6.40.1975)


2d.) autoResponse.txt
----

autoResponse.txt maintains the template for the body of the e-mail receipt that is sent to the end-user, 
provided autoRespond is turned on in config.txt and the user has provided an e-mail address.
You can edit this as you like, and variables within it will be automatically replaced at run-time - you may re-use, or not use, variables as you see fit.
In an effort to maintain simplicity of function and support all possible e-mail clients, the formatting preserves line-breaks, but does not maintain other formatting.

variables:

<datetime> - replaced with the current server date and time in MMM ddd DD HH:MM YYYY format
<contactPref> - based on state of Contact Preference radio button at submit time - "E-mail", "Phone", or "No Preference"
<ticket> - a verbatim copy of the "issue" portion of the ticket sent to the support engineer 

At this time, the other variables included in ticketTemplate.txt are not supported for the autoResponse. Sending ticket details is an all-or-nothing choice.
However, you may omit any of the variables you like, or all of them if you prefer.

3.)Contact information
----

Contact the dev at justin.a@quickpractice.com

4.)Disclaimer
----

This submission is supplied without any WARRANTY (EXPRESSED or IMPLIED)
and is intended in good faith to provide for the usage case listed above.