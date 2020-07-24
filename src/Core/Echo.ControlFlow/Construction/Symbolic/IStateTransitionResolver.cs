﻿using System;
using Echo.DataFlow.Emulation;

namespace Echo.ControlFlow.Construction.Symbolic
{
    /// <summary>
    /// Provides members for resolving the next possible states of a program after the execution of an instruction.
    /// </summary>
    /// <typeparam name="TInstruction">The type of instruction that is being executed.</typeparam>
    public interface IStateTransitionResolver<TInstruction>
    {
        /// <summary>
        /// Gets the initial state of the program at a provided entry point address.
        /// </summary>
        /// <param name="entrypointAddress">The entry point address.</param>
        /// <returns>The object representing the initial state of the program.</returns>
        SymbolicProgramState<TInstruction> GetInitialState(long entrypointAddress);

        int GetTransitionCount(SymbolicProgramState<TInstruction> currentState, in TInstruction instruction);
        
        /// <summary>
        /// Resolves all possible program state transitions that the provided instruction can apply. 
        /// </summary>
        /// <param name="currentState">The current state of the program.</param>
        /// <param name="instruction">The instruction to evaluate.</param>
        /// <param name="successorBuffer"></param>
        /// <returns>A collection of state transitions that the provided instruction might apply.</returns>
        int GetTransitions(
            SymbolicProgramState<TInstruction> currentState, 
            in TInstruction instruction, 
            Span<StateTransition<TInstruction>> successorBuffer);
    }
}