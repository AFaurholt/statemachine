//-------------------------------------------------------------------------------
// <copyright file="Initialization.cs" company="Appccelerate">
//   Copyright (c) 2008-2017 Appccelerate
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Appccelerate.StateMachine.Sync
{
    using System;
    using System.Collections.Generic;
    using Appccelerate.StateMachine.Infrastructure;
    using Appccelerate.StateMachine.Machine;
    using Appccelerate.StateMachine.Persistence;
    using FakeItEasy;
    using FluentAssertions;
    using Specs;
    using Xbehave;

    public class Initialization
    {
        private const int TestState = 1;

        private readonly CurrentStateExtension testExtension = new CurrentStateExtension();

        [Scenario]
        public void Start(
            PassiveStateMachine<int, int> machine,
            bool entryActionExecuted)
        {
            "establish an initialized state machine".x(() =>
                {
                    machine = new PassiveStateMachine<int, int>();

                    machine.AddExtension(this.testExtension);

                    machine.In(TestState)
                        .ExecuteOnEntry(() => entryActionExecuted = true);

                    machine.Initialize(TestState);
                });

            "when starting the state machine".x(() =>
                machine.Start());

            "should set current state of state machine to state to which it is initialized".x(() =>
                this.testExtension.CurrentState.Should().Be(TestState));

            "should execute entry action of state to which state machine is initialized".x(() =>
                entryActionExecuted.Should().BeTrue());
        }

        [Scenario]
        public void StartForStateMachineBuiltByStateMachineDefinitionBuilder(
            PassiveStateMachine<int, int> machine,
            bool entryActionExecuted)
        {
            "establish an initialized state machine".x(() =>
            {
                machine = new StateMachineDefinitionBuilder<int, int>()
                    .WithConfiguration(x =>
                        x.In(TestState)
                            .ExecuteOnEntry(() => entryActionExecuted = true))
                    .Build()
                    .CreatePassiveStateMachine();

                machine.AddExtension(this.testExtension);

                machine.Initialize(TestState);
            });

            "when starting the state machine".x(() =>
                machine.Start());

            "should set current state of state machine to state to which it is initialized".x(() =>
                this.testExtension.CurrentState.Should().Be(TestState));

            "should execute entry action of state to which state machine is initialized".x(() =>
                entryActionExecuted.Should().BeTrue());
        }

        [Scenario]
        public void Initialize(
            PassiveStateMachine<int, int> machine,
            bool entryActionExecuted)
        {
            "establish a state machine".x(() =>
                {
                    machine = new PassiveStateMachine<int, int>();

                    machine.AddExtension(this.testExtension);

                    machine.In(TestState)
                        .ExecuteOnEntry(() => entryActionExecuted = true);
                });

            "when state machine is initialized".x(() =>
                machine.Initialize(TestState));

            "should not yet execute any entry actions".x(() =>
                entryActionExecuted.Should().BeFalse());
        }

        [Scenario]
        public void Reinitialization(
            PassiveStateMachine<int, int> machine,
            Exception receivedException)
        {
            "establish an initialized state machine".x(() =>
                {
                    machine = new PassiveStateMachine<int, int>();
                    machine.Initialize(TestState);
                });

            "when state machine is initialized again".x(() =>
                {
                    try
                    {
                        machine.Initialize(TestState);
                    }
                    catch (Exception e)
                    {
                        receivedException = e;
                    }
                });

            "should throw an invalid operation exception".x(() =>
                {
                    receivedException
                        .Should().BeAssignableTo<InvalidOperationException>();
                    receivedException.Message
                        .Should().Be(ExceptionMessages.StateMachineIsAlreadyInitialized);
                });
        }

        [Scenario]
        public void StartingAnUninitializedStateMachine(
            PassiveStateMachine<int, int> machine,
            Exception receivedException)
        {
            "establish an uninitialized state machine".x(() =>
                {
                    machine = new PassiveStateMachine<int, int>();
                });

            "when starting the state machine".x(() =>
                receivedException = Catch.Exception(() =>
                    machine.Start()));

            "should throw an invalid operation exception".x(() =>
                {
                    receivedException
                        .Should().BeAssignableTo<InvalidOperationException>();
                    receivedException.Message
                        .Should().Be(ExceptionMessages.StateMachineNotInitialized);
                });
        }

        [Scenario]
        public void InitializeALoadedStateMachine(
            PassiveStateMachine<int, int> machine,
            Exception receivedException)
        {
            "establish a loaded initialized state machine".x(() =>
                {
                    machine = new PassiveStateMachine<int, int>();

                    var loader = new Persisting.StateMachineLoader<int>();
                    loader.SetCurrentState(new Initializable<int> { Value = 1 });
                    loader.SetHistoryStates(new Dictionary<int, int>());
                    machine.Load(loader);
                });

            "when initializing the state machine".x(() =>
                    receivedException = Catch.Exception(() =>
                        machine.Initialize(0)));

            "should throw an invalid operation exception".x(() =>
                {
                    receivedException
                        .Should().BeAssignableTo<InvalidOperationException>()
                        .Which.Message
                        .Should().Be(ExceptionMessages.StateMachineIsAlreadyInitialized);
                });
        }
    }
}