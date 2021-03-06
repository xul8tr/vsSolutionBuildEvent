﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass]
    public class BoxComponentTest
    {
        [TestMethod]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest1()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            target.parse(@"#[Box notrealnode]");
        }

        [TestMethod]
        public void repeatTest1()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            Assert.AreEqual("ab!ab!ab!ab!", noSpaces(@"
                                                #[$(i = 0)]#[Box repeat($(i) < 4): $(i = $([MSBuild]::Add($(i), 1)))
                                                    ab!
                                                ]", target));

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("4", uvar.get("i", null));
        }

        [TestMethod]
        public void repeatTest2()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            Assert.AreEqual("ab!ab!ab!", noSpaces(@"
                                                #[$(i = 2)]#[Box repeat($(i) < 8): $(i = $([MSBuild]::Add($(i), 2)))
                                                    ab!
                                                    #[Box operators.sleep(50)]
                                                ]", target));

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("8", uvar.get("i", null));
        }

        [TestMethod]
        public void repeatTest3()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            Assert.AreEqual(String.Empty, noSpaces(@"
                                                #[$(i = 2)]#[Box repeat($(i) < 8; true): $(i = $([MSBuild]::Add($(i), 1)))
                                                    ab!
                                                ]", target));

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("8", uvar.get("i", null));
        }

        [TestMethod]
        public void repeatTest4()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            Assert.AreEqual("ab!ab!ab!ab!", noSpaces(@"
                                                #[$(i = 0)]#[Box repeat($(i) < 4; false): $(i = $([MSBuild]::Add($(i), 1)))
                                                    ab!
                                                ]", target));

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("4", uvar.get("i", null));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void repeatTest5()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            noSpaces(@"#[Box repeat(false; false; true): ]", target);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void repeatTest6()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            noSpaces(@"#[Box repeat( ): ]", target);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void repeatTest7()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            noSpaces(@"#[Box repeat(false; 123): ]", target); // int type instead of bool
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void repeatTest8()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            noSpaces(@"#[Box repeat: ]", target);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void iterateTest1()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            noSpaces(@"#[Box iterate: ]", target);
        }

        [TestMethod]
        public void iterateTest2()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            Assert.AreEqual("ab!ab!ab!ab!", noSpaces(@"
                                                #[$(i = 0)]#[Box iterate(;$(i) < 4; ): $(i = $([MSBuild]::Add($(i), 1)))
                                                    ab!
                                                ]", target));

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("4", uvar.get("i", null));
        }

        [TestMethod]
        public void iterateTest3()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            Assert.AreEqual("ab!ab!ab!", noSpaces(@"
                                                #[$(i = -2)]#[Box iterate(;$(i) < 4; i = $([MSBuild]::Add($(i), 2))): 
                                                    ab!
                                                ]", target));

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("4", uvar.get("i", null));
        }

        [TestMethod]
        public void iterateTest4()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            Assert.AreEqual("ab!ab!ab!", noSpaces(@"
                                                #[Box iterate(i = 1; $(i) < 4; i = $([MSBuild]::Add($(i), 1))): 
                                                    ab!
                                                ]", target));

            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("4", uvar.get("i", null));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void iterateTest5()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            target.parse(@"#[Box iterate(i = 1; $(i) < 4; i = $([MSBuild]::Add($(i), 1)); ): ]");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void iterateTest6()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            target.parse(@"#[Box iterate(; i = 1; $(i) < 4; i = $([MSBuild]::Add($(i), 1)) ): ]");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void opSleepTest1()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            target.parse(@"#[Box operators.sleep()]");
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void opSleepTest2()
        {
            var target = new Script(new StubEnv(), new UserVariable());
            target.parse(@"#[Box operators.notrealProperty]");
        }

        [TestMethod]
        public void opSleepTest3()
        {
            var target = new Script(new StubEnv(), new UserVariable());

            var start = DateTime.Now;
            target.parse(@"#[Box operators.sleep(1000)]");
            var end = DateTime.Now;

            Assert.IsTrue((end - start).TotalMilliseconds >= 1000);
        }

        [TestMethod]
        public void stDataTest1()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            uvar.set("p1", null, "v1");
            uvar.evaluate("p1", null, new EvaluatorBlank(), true);

            Assert.AreEqual(String.Empty, target.parse("#[Box data.pack(\"test1\", false): $(p1)#[Box operators.sleep(10)] ]"));
            Assert.AreEqual(String.Empty, target.parse("#[Box data.pack(\"test2\", true): $(p1) #[Box operators.sleep(10)] ]"));

            Assert.AreEqual("$(p1)#[Box operators.sleep(10)]", target.parse("#[Box data.get(\"test1\", false)]").Trim());
            Assert.AreEqual("v1", noSpaces("#[Box data.get(\"test1\", true)]", target));

            Assert.AreEqual("v1", noSpaces("#[Box data.get(\"test2\", false)]", target));
            Assert.AreEqual("v1", noSpaces("#[Box data.get(\"test2\", true)]", target));
        }

        [TestMethod]
        [ExpectedException(typeof(LimitException))]
        public void stDataTest2()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            target.parse("#[Box data.pack(\"test1\", false): 123]");
            target.parse("#[Box data.pack(\"test1\", true): 123]");
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void stDataTest3()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            target.parse("#[Box data.get(\"notexists\", false)]");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedOperationException))]
        public void stDataTest4()
        {
            var uvar = new UserVariable();
            var target = new Script(new StubEnv(), uvar);

            target.parse("#[Box data.get(\"test1\", false): 123]");
        }

        [TestMethod]
        public void stDataTest5()
        {
            var uvar = new UserVariable();
            var target = new Script(new StubEnv(), uvar);

            uvar.set("p1", null, "ab!");
            uvar.evaluate("p1", null, new EvaluatorBlank(), true);

            Assert.AreEqual(String.Empty, target.parse("#[Box data.pack(\"test1\", false): $(p1) ]"));
            Assert.AreEqual(String.Empty, target.parse("#[Box data.pack(\"test2\", true): $(p1) ]"));

            Assert.AreEqual("$(p1)$(p1)$(p1)$(p1)", noSpaces("#[Box data.clone(\"test1\", 4)]", target));
            Assert.AreEqual("$(p1)$(p1)$(p1)$(p1)", noSpaces("#[Box data.clone(\"test1\", 4, false)]", target));
            Assert.AreEqual("ab!ab!ab!ab!", noSpaces("#[Box data.clone(\"test1\", 4, true)]", target));

            Assert.AreEqual("ab!ab!", noSpaces("#[Box data.clone(\"test2\", 2)]", target));
            Assert.AreEqual("ab!ab!", noSpaces("#[Box data.clone(\"test2\", 2, false)]", target));
            Assert.AreEqual("ab!ab!", noSpaces("#[Box data.clone(\"test2\", 2, true)]", target));

            Assert.AreEqual(String.Empty, noSpaces("#[Box data.clone(\"test2\", 0)]", target));
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void stDataTest6()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);
            
            target.parse("#[Box data.clone(\"notexists\", 4)]");
        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void stDataTest7()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);
            
            Assert.AreEqual(String.Empty, target.parse("#[Box data.pack(\"test1\", false): 123 ]"));
            Assert.AreEqual(String.Empty, target.parse("#[Box data.free(\"test1\")]"));

            target.parse("#[Box data.get(\"test1\", false)]");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedOperationException))]
        public void stDataTest8()
        {
            var uvar    = new UserVariable();
            var target  = new Script(new StubEnv(), uvar);

            target.parse("#[Box data.free(\"test1\"): 123]");
        }

        [TestMethod]
        public void stDataTest9()
        {
            var uvar = new UserVariable();
            var target = new Script(new StubEnv(), uvar);

            Assert.AreEqual(String.Empty, target.parse("#[Box data.free(\"test1\")]"));
            Assert.AreEqual(String.Empty, target.parse("#[Box data.free(\"test2\")]"));
        }

        private string noSpaces(string raw, ISBEScript script)
        {
            return script.parse(raw).Replace("\r", "").Replace("\n", "").Replace(" ", "");
        }
    }
}
