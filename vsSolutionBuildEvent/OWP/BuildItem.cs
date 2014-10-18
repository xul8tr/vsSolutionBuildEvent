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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.vsSBE.OWP
{
    /// <summary>
    /// Working with the "Build"
    /// http://msdn.microsoft.com/en-us/library/yxkt8b26%28v=vs.120%29.aspx
    /// </summary>
    public class BuildItem
    {
        public int ErrorsCount
        {
            get { return errors.Count; }
        }

        public int WarningsCount
        {
            get { return warnings.Count; }
        }

        public bool IsErrors
        {
            get { return ErrorsCount > 0; }
        }

        public bool IsWarnings
        {
            get { return WarningsCount > 0; }
        }

        /// <summary>
        /// all errors in partial data
        /// </summary>
        public List<string> Errors
        {
            get { return errors; }
        }
        protected List<string> errors = new List<string>();

        /// <summary>
        /// all warnings in partial data
        /// </summary>
        public List<string> Warnings
        {
            get { return warnings; }
        }
        protected List<string> warnings = new List<string>();

        public enum Type
        {
            Warnings,
            Errors
        }

        /// <summary>
        /// Current raw
        /// </summary>
        protected string rawdata;

        /// <summary>
        /// Updating data and immediately extracting contents
        /// </summary>
        /// <param name="rawdata"></param>
        public void updateRaw(string rawdata)
        {
            this.rawdata = rawdata;
            extract();
        }

        public bool checkRule(Type type, bool isWhitelist, List<string> codes)
        {
            if(isWhitelist) {
                if((codes.Count < 1 && (type == Type.Warnings ? Warnings : Errors).Count > 0) || 
                    (codes.Count > 0 && codes.Intersect(type == Type.Warnings ? Warnings : Errors).Count() > 0)) {
                    return true;
                }
                return false;
            }

            if(codes.Count < 1) {
                return false;
            }
            if((type == Type.Warnings ? Warnings : Errors).Except(codes).Count() > 0) {
                return true;
            }
            return false;
        }

        protected void extract()
        {
            MatchCollection matches = Regex.Matches(rawdata, @":\s*?(error|warning)([^:]+):", RegexOptions.IgnoreCase);
            // 1  -> type
            // 2  -> code####

            foreach(Match m in matches){
                if(!m.Success){
                    continue;
                }

                string code = m.Groups[2].Value.Trim();
                switch(m.Groups[1].Value)
                {
                    case "error": { errors.Add(code); break; }
                    case "warning": { warnings.Add(code); break; }
                }
            }
        }
    }
}
