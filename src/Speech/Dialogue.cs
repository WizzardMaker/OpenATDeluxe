using Godot;
using System;
using System.Collections.Generic;

/*
    Needs:
    -Branching on condition

*/

public class DialogueNode {
	protected Dialogue master;

	public int textId;

	public List<DialogueOption> options = new List<DialogueOption>();

	protected Action onSpeechFinished, start;

	public DialogueNode(int textId) {
		this.textId = textId;
	}

	public virtual void Start(Dialogue master) {
		this.master = master;

		start?.Invoke();
	}
	public virtual void OnSpeechFinished() {
		onSpeechFinished?.Invoke();
	}

	public void AddOption(DialogueOption option) {
		options.Add(option);
	}
}

public class DialogueNodeReturning : DialogueNode {
	public DialogueNodeReturning(int textId) : base(textId) {
	}

	public override void OnSpeechFinished() {
		base.OnSpeechFinished();

		master.ReturnToPrevNode(); //We are done here... maybe. TODO: Check if the game returns to prev. node when they finished talking
	}
}

public class DialogueOption {
	public int textId;

	DialogueNode destination;

	public DialogueOption(int textId, DialogueNode destination) {
		this.textId = textId;
		this.destination = destination;
	}

	public virtual DialogueNode GetDestinationNode() {
		throw new NotImplementedException();
	}
}

public class DialogueOptionConditioned : DialogueOption {
	public Func<DialogueNode> condition;

	public DialogueOptionConditioned(int textId, Func<DialogueNode> destinations) : base(textId, null) {
		this.condition = destinations;
	}

	public override DialogueNode GetDestinationNode() {
		throw new NotImplementedException();
	}
}

public class Dialogue {
	public string dialogueGroup;

	DialogueNode _currentNode;
	public DialogueNode CurrentNode { get => _currentNode; private set => _currentNode = value; }
	public int CurrentNodeIndex { get => nodes.IndexOf(_currentNode); }

	// Used to return to previous dialogue nodes
	Stack<DialogueNode> dialogueStack = new Stack<DialogueNode>();
	List<DialogueNode> nodes = new List<DialogueNode>();

	public Dialogue(string dialogueGroup) {
		this.dialogueGroup = dialogueGroup;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="node"></param>
	/// <returns>Itself, good for stacking</returns>
	public Dialogue AddNode(DialogueNode node) {
		nodes.Add(node);

		return this;
	}

	/// <summary>
	/// Starts the node which got added first.
	/// </summary>
	public void Start() {
		StartNode(nodes[0]);
	}

	public void StartNode(DialogueNode node, bool addCurrentNodeToStack = true) {
		if (!nodes.Contains(node))
			throw new ArgumentOutOfRangeException("Node not inside Dialogue!");

		if (CurrentNode != null && addCurrentNodeToStack) {
			//Add old node to stack
			dialogueStack.Push(CurrentNode);
		}

		CurrentNode = node;

		CurrentNode.Start(this);
	}

	public void ReturnToPrevNode() {
		DialogueNode prev = dialogueStack.Pop();

		StartNode(prev, false); //Opt out of Stack to prevent looping forth and back
	}

	public void SelectOption(int id) {
		//TODO: Add OOR check!
		DialogueNode nextNode = CurrentNode.options[id].GetDestinationNode();

		StartNode(nextNode);
	}


	// class Option {
	// 	int optionId;

	// 	int leadsToId;
	// 	Dialogue leadsToDialogue;

	// 	Func<int> conditionalId;
	// 	Func<Dialogue> conditionalDialogue;

	// 	Action onPick;

	// 	public bool isAReturningOption;

	// 	public Option(int optionId, Func<int> conditionalId, Action onPick, bool isAReturningOption = true) {
	// 		this.optionId = optionId;
	// 		this.conditionalId = conditionalId;
	// 		this.onPick = onPick;
	// 		this.isAReturningOption = isAReturningOption;
	// 	}

	// 	public Option(int optionId, Func<Dialogue> conditionalDialogue) {
	// 		this.optionId = optionId;
	// 		this.conditionalDialogue = conditionalDialogue;
	// 	}
	// }


	// /// <summary>
	// /// 
	// /// </summary>
	// /// <param name="dialogueGroup">Like "Bank", or "Makl".
	// /// The first part of a localized string ("xxx>1000" - the xxx part).</param>
	// /// <param name="id">The id number of the dialogue option ("xxx>1000" - the number part)</param>
	// public Dialogue(string dialogueGroup, int id) {

	// }

	// /// <summary>
	// /// Add dialogue option, which just answers and then goes right back to this dialogue part
	// /// </summary>
	// /// <param name="id">String id for dialogue option</param>
	// /// <param name="answer">String id for answer from dialogue partner</param>
	// /// <param name="onPick">Gets called when we pick this option</param>
	// public void AddAnswerOptionReturning(int id, int answer, Action onPick = null) {

	// }

	// public void AddAnswerOption(int id, Dialogue leadingTo) {

	// }

	// /// <summary>
	// /// Returns to the previous option
	// /// </summary>
	// /// <param name="id"></param>
	// public void AddAnswerOptionLeave(int id) {

	// }

	// public void AddConditionalAnswerOption(int id, Func<int> leadingTo, Action onPick = null) {

	// }
	// public void AddConditionalAnswerOption(int id, Func<Dialogue> leadingTo) {

	// }
}