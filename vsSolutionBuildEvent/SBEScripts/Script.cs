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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class Script: ISBEScript
    {
        /// <summary>
        /// Definitions of user-variable
        /// </summary>
        protected ConcurrentDictionary<string, string> definitions = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Getting user-defined variable
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <returns>evaluated value of variable or null if variable not defined</returns>
        public string getVariable(string name, string project = null)
        {
            string defindex = String.Format("{0}:{1}", name, project);

            if(!definitions.ContainsKey(defindex)) {
                return null;
            }
            return definitions[defindex];
        }

        /// <summary>
        /// Define user-variable
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name or null if project is default</param>
        /// <param name="value">mixed string. Converted to empty string if value is null</param>
        public void setVariable(string name, string project, string value)
        {
            if(value == null) {
                value = String.Empty;
            }
            string defindex = String.Format("{0}:{1}", name, project);

            Log.nlog.Debug("User-variable: define '{0}' = '{1}'", defindex, value);
            definitions[defindex] = value;
        }

        /// <summary>
        /// Remove user-variable
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <exception cref="ArgumentNullException">key is null</exception>
        public void unsetVariable(string name, string project)
        {
            string res;
            if(!definitions.TryRemove(name, out res)) {
                Log.nlog.Debug("Cannot unset the user-variable '{0}'", name);
                return;
            }
            Log.nlog.Debug("User-variable is successfully unset '{0}'", name);
        }

        /// <summary>
        /// Remove all user-variables
        /// </summary>
        public void unsetVariables()
        {
            definitions.Clear();
            Log.nlog.Debug("All User-variables is successfully reseted");
        }

        /// <summary>
        /// Exposes the enumerator for defined names of user-variables
        /// </summary>
        public IEnumerable<string> Variables
        {
            get {
                foreach(KeyValuePair<string, string> def in definitions) {
                    yield return def.Key;
                }
            }
        }

        /// <summary>
        /// Handler of mixed data SBE-Scripts
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared & evaluated data</returns>
        public string parse(string data)
        {
            //TODO:
            throw new NotImplementedException();
        }

    }
}
