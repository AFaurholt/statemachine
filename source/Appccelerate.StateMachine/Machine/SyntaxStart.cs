namespace Appccelerate.StateMachine.Machine
{
    using System;
    using System.Collections.Generic;
    using Events;
    using States;
    using Syntax;

    public class SyntaxStart<TState, TEvent> : ISyntaxStart<TState, TEvent>
        where TState : IComparable
        where TEvent : IComparable
    {
        private readonly IFactory<TState, TEvent> factory;
        private readonly IStateDictionary<TState, TEvent> stateDefinitionDictionary;
        private readonly IDictionary<TState, IStateDefinition<TState, TEvent>> initiallyLastActiveStates;

        public SyntaxStart(
            IFactory<TState, TEvent> factory,
            IStateDictionary<TState, TEvent> stateDefinitionDictionary,
            IDictionary<TState, IStateDefinition<TState, TEvent>> initiallyLastActiveStates)
        {
            this.factory = factory;
            this.stateDefinitionDictionary = stateDefinitionDictionary;
            this.initiallyLastActiveStates = initiallyLastActiveStates;
        }

        public IEntryActionSyntax<TState, TEvent> In(TState stateId)
        {
            return new StateBuilder<TState, TEvent>(stateId, this.stateDefinitionDictionary, this.factory);
        }

        public IHierarchySyntax<TState> DefineHierarchyOn(TState superStateId)
        {
            return new HierarchyBuilder<TState, TEvent>(superStateId, this.stateDefinitionDictionary, this.initiallyLastActiveStates);
        }
    }
}