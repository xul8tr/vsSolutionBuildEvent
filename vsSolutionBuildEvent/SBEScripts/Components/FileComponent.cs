﻿/* 
 * Boost Software License - Version 1.0 - August 17th, 2003
 * 
 * Copyright (c) 2013-2014 Developed by reg [Denis Kuzmin] <entry.reg@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person or organization
 * obtaining a copy of the software and accompanying documentation covered by
 * this license (the "Software") to use, reproduce, display, distribute,
 * execute, and transmit the Software, and to prepare derivative works of the
 * Software, and to permit third-parties to whom the Software is furnished to
 * do so, all subject to the following:
 * 
 * The copyright notices in the Software and this entire statement, including
 * the above license grant, this restriction and the following disclaimer,
 * must be included in all copies of the Software, in whole or in part, and
 * all derivative works of the Software, unless such copies or derivative
 * works are solely in the form of machine-executable object code generated by
 * a source language processor.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
 * SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
 * FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE. 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    public class FileComponent: IComponent
    {
        /// <summary>
        /// Type of implementation
        /// </summary>
        public ComponentType Type
        {
            get { return ComponentType.Internal; }
        }

        /// <summary>
        /// Handling with current type
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[File
                                              \s+
                                              (                  #1 - full ident
                                                ([A-Za-z_0-9]+)  #2 - subtype
                                                .*
                                              )
                                           \]$", 
                                           RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed FileComponent - '{0}'", data);
            }
            string ident = m.Groups[1].Value;

            switch(m.Groups[2].Value) {
                case "get": {
                    Log.nlog.Debug("FileComponent: use stGet");
                    return stGet(ident);
                }
                case "call": {
                    Log.nlog.Debug("FileComponent: use stCall");
                    return stCall(ident, false);
                }
                case "callOut": {
                    Log.nlog.Debug("FileComponent: use stCall");
                    return stCall(ident, true);
                }
                case "write": {
                    Log.nlog.Debug("FileComponent: use stWrite");
                    stWrite(ident, false, false);
                    return String.Empty;
                }
                case "append": {
                    Log.nlog.Debug("FileComponent: use stWrite + append");
                    stWrite(ident, true, false);
                    return String.Empty;
                }
                case "writeLine": {
                    Log.nlog.Debug("FileComponent: use stWrite + line");
                    stWrite(ident, false, true);
                    return String.Empty;
                }
                case "appendLine": {
                    Log.nlog.Debug("FileComponent: use stWrite + append + line");
                    stWrite(ident, true, true);
                    return String.Empty;
                }
            }
            throw new SubtypeNotFoundException("FileComponent: not found subtype - '{0}'", m.Groups[2].Value);
        }

        /// <summary>
        /// Work with:
        /// * #[File get("name")]
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <returns>Received data from  file</returns>
        protected string stGet(string data)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"get
                                                    \s*
                                                    \({0}\)   #1 - file", 
                                                    RPattern.DoubleQuotesContent
                                                  ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new TermNotFoundException("Failed stGet - '{0}'", data);
            }

            string file     = m.Groups[1].Value.Trim();
            string content  = "";
            try {
                using(StreamReader stream = new StreamReader(file, Encoding.UTF8, true)) {
                    content = stream.ReadToEnd();
                }
                Log.nlog.Debug("FileComponent: successful stGet- '{0}'", file);
            }
            catch(FileNotFoundException exNotFound) {
                Log.nlog.Warn("stGet: not found - '{0}' :: {1}", file, exNotFound.Message);
            }
            catch(Exception ex) {
                Log.nlog.Warn("stGet: exception - '{0}'", ex.Message);
            }
            return content;
        }

        /// <summary>
        /// Work with:
        /// * #[File call("name", "args")]
        /// * #[File call("name")]
        /// * #[File callOut("name", "args")]
        /// * #[File callOut("name")]
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <param name="stdOut">Use StandardOutput or not</param>
        /// <returns>Received data from StandardOutput</returns>
        protected string stCall(string data, bool stdOut)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"
                                                    \s*
                                                    \(
                                                        {0}           #1 - file
                                                        (?:
                                                            \s*,\s*
                                                            {0}       #2 - args (optional)
                                                        )?
                                                    \)", 
                                                    RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new TermNotFoundException("Failed stCall - '{0}'", data);
            }

            string file = m.Groups[1].Value.Trim();
            string args = m.Groups[2].Value;

            if(!Directory.Exists(file)) {
                Log.nlog.Warn("stCall: not found - '{0}'", file);
                return String.Empty;
            }

            try {
                Process p = new Process();
                p.StartInfo.FileName = file;

                p.StartInfo.Arguments               = args;
                p.StartInfo.UseShellExecute         = false;
                p.StartInfo.RedirectStandardOutput  = true;
                p.StartInfo.RedirectStandardError   = true;
                p.Start();

                string errors = p.StandardError.ReadToEnd();
                if(errors.Length > 0) {
                    throw new Exception(errors);
                }
                Log.nlog.Debug("FileComponent: successful stCall - '{0}'", file);
                return (stdOut)? p.StandardOutput.ReadLine() : String.Empty;
            }
            catch(Exception ex) {
                Log.nlog.Warn("stCall: exception - '{0}'", ex.Message);
            }
            return String.Empty;
        }

        /// <summary>
        /// Work with:
        /// * #[File write("name"): "multiline data"]
        /// * #[File append("name"): "multiline data"]
        /// * #[File writeLine("name"): "multiline data"]
        /// * #[File appendLine("name"): "multiline data"]
        /// </summary>
        /// <param name="data">prepared data</param>
        /// <param name="append">flag</param>
        /// <param name="writeLine">writes with CR?/LF</param>
        /// <param name="enc">Used encoding</param>
        protected void stWrite(string data, bool append, bool writeLine, Encoding enc)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"
                                                    \s*
                                                    \({0}\)  #1 - path
                                                    \s*:\s*
                                                    {0}      #2 - data", 
                                                    RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            if(!m.Success) {
                throw new TermNotFoundException("Failed stWrite - '{0}'", data);
            }

            string path     = m.Groups[1].Value.Trim();
            string fdata    = m.Groups[2].Value;

            Log.nlog.Debug("FileComponent: stWrite started for path - '{0}'", path);
            try {
                using(TextWriter stream = new StreamWriter(path, append, enc)) {
                    if(writeLine){
                        stream.WriteLine(fdata);
                    }
                    else{
                        stream.Write(fdata);
                    }
                }
                Log.nlog.Debug("FileComponent: successful stWrite - '{0}'", path);
            }
            catch(Exception ex) {
                Log.nlog.Warn("FileComponent: Cannot write {0}", ex.Message);
            }
        }

        protected void stWrite(string data, bool append, bool writeLine)
        {
            stWrite(data, append, writeLine, Encoding.UTF8);
        }
    }
}
