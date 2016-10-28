/** 
* Copyright (c) 2007, Ritu Jain, Chinmay Nagarkar and Sahi Technologies Pvt Ltd
* All rights reserved.
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the Sahi Technologies Pvt. Ltd./Esahi.com  nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using NUnit.Framework;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Text;
using org.drools.dotnet.compiler;
using org.drools.dotnet.rule;
using org.drools.dotnet;



namespace languageParser
{

    [TestFixture]
    public class Parser
    {
        //string inputMsg;
        //string outputMsg;
        // RuleBase rb;
        PackageBuilder builder;
        System.IO.Stream stream;
        Package pkg;
        RuleBase ruleBase;
        Message message;

        public Parser()
        {

            builder = new PackageBuilder();
            stream = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream("Sensip.Language.LanguageRules.drl");
            builder.AddPackageFromDrl("Language.LanguageRules.drl", stream);
            pkg = builder.GetPackage();
            ruleBase = RuleBaseFactory.NewRuleBase();
            ruleBase.AddPackage(pkg);
            message = new Message();
        }

        public void setInput(string msg)
        {
            message.setInputMsg = msg;
        }
        public string getOutput()
        {
            return message.setOutputMsg;
        }

        public void doParsing()
        {
            //go !
            WorkingMemory workingMemory = ruleBase.NewWorkingMemory();
            //Message message = new Message();

            //message.status = Message.GOODBYE;
            workingMemory.assertObject(message);
            workingMemory.fireAllRules();
        }

    }

    public class Message
    {
        protected string inputMsg;
        protected string ouputMsg;


        //private string messageStr;

        //private int statusInt;

        public string setInputMsg
        {
            get { return inputMsg; }
            set
            {
                inputMsg = value;
            }
        }

        public string setOutputMsg
        {
            get { return ouputMsg; }
            set
            {
                ouputMsg = value;
            }
        }

        /*public int status
        {
            get { return statusInt; }
            set
            {
                statusInt = value;
            }
        }*/

    }


    /*public class Message
    {
        public static int HELLO = 0;
        public static int GOODBYE = 1;


        private string messageStr;

        private int statusInt;

        public string message
        {
            get { return messageStr; }
            set
            {
                messageStr = value;
            }
        }


        public int status
        {
            get { return statusInt; }
            set
            {
                statusInt = value;
            }
        }

    }*/
}


