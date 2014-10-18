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

namespace net.r_eg.vsSBE.MSBuild
{
    public struct TPreparedData
    {
        /// <summary>
        /// Dynamically define variables
        /// </summary>
        public TVar variable;
        /// <summary>
        /// Unit of properties
        /// </summary>
        public TProperty property;

        public struct TVar
        {
            /// <summary>
            /// Storage if present
            /// </summary>
            public string name;
            /// <summary>
            /// Specific project where to store.
            /// null value - project by default
            /// </summary>
            public string project;
            /// <summary>
            /// Storing in the projects files ~ .csproj, .vcxproj, ..
            /// </summary>
            /// <remarks>reserved</remarks>
            public bool isPersistence;
        }

        public struct TProperty
        {
            /// <summary>
            /// Complex phrase or simple property
            /// </summary>
            public bool complex;
            /// <summary>
            /// has escaped property
            /// </summary>
            public bool escaped;
            /// <summary>
            /// Contain all prepared data from specific projects. Not complete evaluation!
            /// i.e.: prepares only all $(..:project) data
            /// </summary>
            public string unevaluated;
            /// <summary>
            /// Specific project for unevaluated data
            /// </summary>
            public string project;
            /// <summary>
            /// Step of handling
            /// </summary>
            public bool completed;
            /// <summary>
            /// Raw unprepared data without(in any case) storage variable
            /// </summary>
            public string raw;
            /// <summary>
            /// Contains analysis of nested data
            /// </summary>
            public Nested nested;
        }

        public struct Nested
        {
            /// <summary>
            /// Intermediate data of analysis
            /// </summary>
            public string data;
            /// <summary>
            /// Unevaluated values for present placeholders in data
            /// </summary>
            public Dictionary<int, List<Node>> nodes;
            /// <summary>
            /// true value if one or more nodes contains the Property
            /// </summary>
            public bool hasProperty;

            public struct Node
            {
                /// <summary>
                /// mixed data of arguments
                /// e.g.: simple string or unevaluated complex property
                /// </summary>
                public string data;
                /// <summary>
                /// Specific project if exist or null
                /// </summary>
                public string project;
                /// <summary>
                /// Support for variable of variable.
                /// Contains evaluated data or escaped property (without escape symbol).
                /// the null value if disabled evaluation (e.g. string argument)
                /// </summary>
                public string evaluated;
                /// <summary>
                /// Index of previous node for Left operand
                /// >= 0 / -1 if not used
                /// </summary>
                public int backLinkL;
                /// <summary>
                /// Index of previous node for Right operand
                /// >= 0 / -1 if not used
                /// </summary>
                public int backLinkR;

                public TypeValue type;

                public Node(string data, TypeValue type = TypeValue.Unknown, string project = null, string evaluated = null)
                {
                    this.data       = data;
                    this.project    = project;
                    this.evaluated  = evaluated;
                    this.type       = type;
                    this.backLinkL  = -1;
                    this.backLinkR  = -1;
                }
            }

            public enum TypeValue
            {
                Unknown,
                Property,
                PropertyEscaped,
                String
            }
        }
    }
}
