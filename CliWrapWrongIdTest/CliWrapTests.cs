using System;
using System.Diagnostics;
using System.Reactive.Linq;
using CliWrap;
using CliWrap.EventStream;
using NUnit.Framework;

namespace CliWrapWrongIdTest
{
    public class CliWrapTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CliWrap_process_ID_is_zero_is_wrong_behaviour_and_should_fail()
        {
            var id = -1;

            string ffmpegPath = "...";
            
            // I'm not expert in reactive programming but I think next line should fail
            // because during subscription the command is executed.
            // But I didn't find any check for executable path in your code

            var cmd = Cli.Wrap(ffmpegPath);
            
            var observable = cmd.Observe();

            // In my basic reactive knowledge the subscription should not throw any exception 
            // but in case you get this point you should call observer.OnError during observable create

            using var changePriority = observable.Select(x => x as StartedCommandEvent)
                .Where(x => x != null)
                .Take(1)
                .Select(x => x.ProcessId)
                //.Where(x => x > 0)   // This filters the process id only accepting values above zero
                .Subscribe(SetPriorityByProcessId); // Proccess id == 0  then Win32Exception

            void SetPriorityByProcessId(int processId)
            {
                id = processId;
                //var process = Process.GetProcessById(processId);
                //process.PriorityClass = ProcessPriorityClass.BelowNormal;
            }

            Assert.That(id == 0); // this should fail but not, or execution should no even get this point
        }
    }
}