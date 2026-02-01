using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class TabController
{
    public TabController() { }

    public enum RuleActionType
    {
        Show,
        Hide,
        ChangeValue,
    }

    [UnityEngine.SerializeField] List<Rule> _rules = new List<Rule>();

    // Which tab needs the roles to be run when its fields change
    // True means rules need to be run for that field
    Dictionary<TabInput, bool> _fieldsToRulesRun = new Dictionary<TabInput, bool>();

    /// <summary>
    /// <para>Rule:</para>
    /// <para>When [source inputs (index)]->value equal [values to compare (index)]</para>
    /// <para>then forEach[target inputs] do [action]</para>
    /// </summary>
    [Serializable]
    class Rule
    {
        public RuleActionType _type;// { get; private set; }
        public TabInput[] _sourceInputs;// { get; private set; }
        public object[] _valuesToCompare;// { get; private set; }
        public TabInput[] _targetInputs;// { get; private set; }
        public TabInput _targetInput;// { get; private set; }
        public object _valueToChange = null;// { get; private set; }
        public Func<object> _valueToChangeFunc = null;// { get; private set; }

        public Rule() { }

        public Rule InitShow(TabInput[] sourceInputs, object[] valuesToCompare, TabInput[] targetInputs)
        {
            if (sourceInputs.Length != valuesToCompare.Length)
                throw new System.Exception("Source inputs length must be equal to values to compare length.");
            if (targetInputs.Length == 0)
                throw new System.Exception("Target inputs length must be greater than zero.");
            if (sourceInputs.Length == 0)
                throw new System.Exception("Source inputs length must be greater than zero.");
            if (valuesToCompare.Length == 0)
                throw new System.Exception("Values to compare length must be greater than zero.");

            for (int i = 0; i < sourceInputs.Length; i++)
            {
                if (sourceInputs[i] == null)
                    throw new System.Exception("Source input at index " + i + " is null.");
                if (valuesToCompare[i] == null)
                    throw new System.Exception("Value to compare at index " + i + " is null.");
                if (sourceInputs[i].GetMainValueType() != valuesToCompare[i].GetType())
                    throw new System.Exception("Source input type at index " + i + " does not match value to compare type.");
            }

            for (int i = 0; i < targetInputs.Length; i++)
            {
                if (targetInputs[i] == null)
                    throw new System.Exception("Target input at index " + i + " is null.");
            }

            _type = RuleActionType.Show;
            _sourceInputs = sourceInputs;
            _valuesToCompare = valuesToCompare;
            _targetInputs = targetInputs;

            return this;
        }

        public Rule InitHide(TabInput[] sourceInputs, object[] valuesToCompare, TabInput[] targetInputs)
        {
            if (sourceInputs.Length != valuesToCompare.Length)
                throw new System.Exception("Source inputs length must be equal to values to compare length.");
            if (targetInputs.Length == 0)
                throw new System.Exception("Target inputs length must be greater than zero.");
            if (sourceInputs.Length == 0)
                throw new System.Exception("Source inputs length must be greater than zero.");
            if (valuesToCompare.Length == 0)
                throw new System.Exception("Values to compare length must be greater than zero.");

            for (int i = 0; i < sourceInputs.Length; i++)
            {
                if (sourceInputs[i] == null)
                    throw new System.Exception("Source input at index " + i + " is null.");
                if (valuesToCompare[i] == null)
                    throw new System.Exception("Value to compare at index " + i + " is null.");
                if (sourceInputs[i].GetMainValueType() != valuesToCompare[i].GetType())
                    throw new System.Exception("Source input type at index " + i + " does not match value to compare type.");
            }

            for (int i = 0; i < targetInputs.Length; i++)
            {
                if (targetInputs[i] == null)
                    throw new System.Exception("Target input at index " + i + " is null.");
            }

            _type = RuleActionType.Hide;
            _sourceInputs = sourceInputs;
            _valuesToCompare = valuesToCompare;
            _targetInputs = targetInputs;

            return this;
        }

        public Rule InitChangeValue(TabInput[] sourceInputs, object[] valuesToCompare, TabInput targetInput, object valueToChange)
        {
            if (sourceInputs.Length != valuesToCompare.Length)
                throw new System.Exception("Source inputs length must be equal to values to compare length.");
            if (sourceInputs.Length == 0)
                throw new System.Exception("Source inputs length must be greater than zero.");
            if (valuesToCompare.Length == 0)
                throw new System.Exception("Values to compare length must be greater than zero.");
            if (targetInput == null)
                throw new System.Exception("Target input is null.");
            if (valueToChange == null)
                throw new System.Exception("Value to change is null.");

            for (int i = 0; i < sourceInputs.Length; i++)
            {
                if (sourceInputs[i] == null)
                    throw new System.Exception("Source input at index " + i + " is null.");
                if (valuesToCompare[i] == null)
                    throw new System.Exception("Value to compare at index " + i + " is null.");
                if (sourceInputs[i].GetMainValueType() != valuesToCompare[i].GetType())
                    throw new System.Exception("Source input type at index " + i + " does not match value to compare type.");
            }
            // UnityEngine.Debug.Log("Target input type: " + targetInput.GetAssignedValueType());
            // UnityEngine.Debug.Log("Value to change type: " + valueToChange.GetType());
            if (targetInput.GetAssignedValueType() != valueToChange.GetType())
                throw new System.Exception("Target input type does not match value to change type.\nTarget input type: " + targetInput.GetAssignedValueType() + "\nValue to change type: " + valueToChange.GetType());

            _type = RuleActionType.ChangeValue;
            _sourceInputs = sourceInputs;
            _valuesToCompare = valuesToCompare;
            _targetInput = targetInput;
            _valueToChange = valueToChange;


            return this;
        }

        public Rule InitChangeValue(TabInput[] sourceInputs, TabInput targetInput, object valueToChange)
        {
            if (sourceInputs.Length == 0)
                throw new System.Exception("Source inputs length must be greater than zero.");
            if (targetInput == null)
                throw new System.Exception("Target input is null.");
            if (valueToChange == null)
                throw new System.Exception("Value to change is null.");

            for (int i = 0; i < sourceInputs.Length; i++)
            {
                if (sourceInputs[i] == null)
                    throw new System.Exception("Source input at index " + i + " is null.");
            }
            // UnityEngine.Debug.Log("Target input type: " + targetInput.GetAssignedValueType());
            //UnityEngine.Debug.Log("Value to change type: " + valueToChange.GetType());
            if (targetInput.GetAssignedValueType() != valueToChange.GetType())
                throw new System.Exception("Target input type does not match value to change type.\nTarget input type: " + targetInput.GetAssignedValueType() + "\nValue to change type: " + valueToChange.GetType());

            _type = RuleActionType.ChangeValue;
            _sourceInputs = sourceInputs;
            _valuesToCompare = null;
            _targetInput = targetInput;
            _valueToChange = valueToChange;

            return this;
        }

        public Rule InitChangeValue(TabInput[] sourceInputs, TabInput targetInput, Func<object> valueToChangeFunc)
        {
            object funcValue = valueToChangeFunc();
            if (sourceInputs.Length == 0)
                throw new System.Exception("Source inputs length must be greater than zero.");
            if (targetInput == null)
                throw new System.Exception("Target input is null.");
            if (valueToChangeFunc == null)
                throw new System.Exception("Value to change is null.");

            for (int i = 0; i < sourceInputs.Length; i++)
            {
                if (sourceInputs[i] == null)
                    throw new System.Exception("Source input at index " + i + " is null.");
            }
            // UnityEngine.Debug.Log("Target input type: " + targetInput.GetAssignedValueType());
            // UnityEngine.Debug.Log("Value to change type: " + funcValue.GetType());
            if (targetInput.GetAssignedValueType() != funcValue.GetType())
                throw new System.Exception("Target input type does not match value to change type.\nTarget input type: " + targetInput.GetAssignedValueType() + "\nValue to change type: " + funcValue.GetType());

            _type = RuleActionType.ChangeValue;
            _sourceInputs = sourceInputs;
            _valuesToCompare = null;
            _targetInput = targetInput;
            _valueToChangeFunc = valueToChangeFunc;

            return this;
        }
    }

    public void RemoveAllRules()
    {
        _rules.Clear();
    }

    // Updates dictionary of fields that need rules to be run when changed
    public void Reload()
    {
        End();
        _fieldsToRulesRun.Clear();
        _rules.SelectMany(r => r._sourceInputs).Distinct().ToList().ForEach(tabInput =>
        {
            //UnityEngine.Debug.Log("Registering field for rules running: " + tabInput.GetFieldName());
            _fieldsToRulesRun.Add(tabInput, true);
            tabInput.OnValueChanged += (input) => _fieldsToRulesRun[input] = true;
        });

        //UnityEngine.Debug.Log($"TabController reloaded. Fields to run rules on change: {_fieldsToRulesRun.Count}");
        /*foreach (var field in _fieldsToRulesRun)
        {
            UnityEngine.Debug.Log($" - Field: {field.Key.GetFieldName()}");
        }*/
    }

    public void End()
    {
        _fieldsToRulesRun.Select(i => i.Key).ToList().ForEach(i =>
        {
            i.DesubscribeAll();
        });
    }

    /// <summary>
    /// <para>-- Rule Template for Show/Hide action --</para>
    /// <para>NewRule(</para>
    /// <para>sourceInputs: new() {inputs},</para>
    /// <para>valuesToCompare: new() {values},</para>
    /// <para>targetInputs: new() {targets},</para>
    /// <para>actionType: RuleActionType.type</para>
    /// <para>);</para>
    /// </summary>
    public void NewRule(TabInput[] sourceInputs, object[] valuesToCompare, TabInput[] targetInputs, RuleActionType actionType)
    {
        if (actionType == RuleActionType.Show)
        {
            Rule rule = new Rule().InitShow(sourceInputs, valuesToCompare, targetInputs);
            _rules.Add(rule);
        }
        else if (actionType == RuleActionType.Hide)
        {
            Rule rule = new Rule().InitHide(sourceInputs, valuesToCompare, targetInputs);
            _rules.Add(rule);
        }
        else throw new System.Exception($"Wrong method syntax for this rule action type: {actionType}");
    }

    /// <summary>
    /// <para>-- Rule Template for ChangeValue action --</para>
    /// <para>NewRule(</para>
    /// <para>sourceInputs: new() {inputs},</para>
    /// <para>valuesToCompare: new() {values},</para>
    /// <para>targetInput: new() {targets},</para>
    /// <para>valueToChange: value</para>
    /// <para>actionType: RuleActionType.type</para>
    /// <para>);</para>
    /// </summary>
    public void NewRule(TabInput[] sourceInputs, object[] valuesToCompare, TabInput targetInput, object valueToChange, RuleActionType actionType)
    {
        if (actionType == RuleActionType.ChangeValue)
        {
            Rule rule = new Rule().InitChangeValue(sourceInputs, valuesToCompare, targetInput, valueToChange);
            _rules.Add(rule);
        }
        else throw new System.Exception($"Wrong method syntax for this rule action type: {actionType}");
    }

    /// <summary>
    /// <para>-- Rule Template for ChangeValue action always when all sources have changed --</para>
    /// <para>NewRule(</para>
    /// <para>sourceInputs: new() {inputs},</para>
    /// <para>targetInput: new() {targets},</para>
    /// <para>valueToChange: value</para>
    /// <para>actionType: RuleActionType.type</para>
    /// <para>);</para>
    /// </summary>
    public void NewRule(TabInput[] sourceInputs, TabInput targetInput, object valueToChange, RuleActionType actionType)
    {
        if (actionType == RuleActionType.ChangeValue)
        {
            Rule rule = new Rule().InitChangeValue(sourceInputs, targetInput, valueToChange);
            _rules.Add(rule);
        }
        else throw new System.Exception($"Wrong method syntax for this rule action type: {actionType}");
    }

    /// <summary>
    /// <para>-- Rule Template for ChangeValue action always when all sources have changed --</para>
    /// <para>NewRule(</para>
    /// <para>sourceInputs: new() {inputs},</para>
    /// <para>targetInput: new() {targets},</para>
    /// <para>valueToChangeFunc: Func<object></para>
    /// <para>actionType: RuleActionType.type</para>
    /// <para>);</para>
    /// </summary>
    public void NewRule(TabInput[] sourceInputs, TabInput targetInput, Func<object> valueToChangeFunc, RuleActionType actionType)
    {
        if (actionType == RuleActionType.ChangeValue)
        {
            Rule rule = new Rule().InitChangeValue(sourceInputs, targetInput, valueToChangeFunc);
            _rules.Add(rule);
        }
        else throw new System.Exception($"Wrong method syntax for this rule action type: {actionType}");
    }

    private bool _canContinueRunningRules = false;
    public void StopRunningRules()
    {
        _canContinueRunningRules = false;
    }

    public IEnumerator RunRules(Action onComplete = null)
    {
        _canContinueRunningRules = true;
        int counter = 0;
        int countToYield = 50; // Number of rules to process before yielding

        foreach (Rule rule in _rules)
        {
            foreach (var _sourceInput in rule._sourceInputs)
            {
                if (_fieldsToRulesRun.ContainsKey(_sourceInput) == false)
                    throw new System.Exception("Source input not registered for rules running: " + rule._sourceInputs[0].GetFieldName() + " Be sure to call TabController.Reload() after adding all rules.");
            }

            bool shouldProcessRule = false;
            foreach (var _sourceInput in rule._sourceInputs)
            {
                if (_fieldsToRulesRun[_sourceInput])
                {
                    shouldProcessRule = true;
                    break;
                }
            }

            if (!shouldProcessRule)
                continue;

            if (!_canContinueRunningRules)
                yield break;

            UnityEngine.Debug.Log("Processing rule: " + counter + " - " + rule._type);

            switch (rule._type)
            {
                case RuleActionType.Show:
                    {
                        bool allMatch = true;
                        for (int i = 0; i < rule._sourceInputs.Length; i++)
                        {
                            if (!rule._sourceInputs[i].GetCurrentValue().Equals(rule._valuesToCompare[i]))
                            {
                                allMatch = false;
                                break;
                            }
                        }

                        if (allMatch)
                        {
                            foreach (var target in rule._targetInputs)
                            {
                                target.gameObject.SetActive(true);
                            }
                        }
                    }
                    break;
                case RuleActionType.Hide:
                    {
                        bool allMatch = true;
                        for (int i = 0; i < rule._sourceInputs.Length; i++)
                        {
                            if (!rule._sourceInputs[i].GetCurrentValue().Equals(rule._valuesToCompare[i]))
                            {
                                allMatch = false;
                                break;
                            }
                        }

                        if (allMatch)
                        {
                            foreach (var target in rule._targetInputs)
                            {
                                target.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
                case RuleActionType.ChangeValue:
                    {
                        bool allMatch = true;
                        if (rule._valuesToCompare != null)
                        {
                            for (int i = 0; i < rule._sourceInputs.Length; i++)
                            {
                                if (!rule._sourceInputs[i].GetCurrentValue().Equals(rule._valuesToCompare[i]))
                                {
                                    allMatch = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            allMatch = true; // Always true if no values to compare
                        }

                        if (allMatch)
                        {
                            if (rule._valueToChange != null)
                                rule._targetInput.SetValue(rule._valueToChange);
                            else if (rule._valueToChangeFunc != null)
                                rule._targetInput.SetValue(rule._valueToChangeFunc());
                            else
                                throw new System.Exception("Both valueToChange and valueToChangeFunc are null in ChangeValue rule.");
                        }
                    }
                    break;
            }

            if (counter++ >= countToYield)
            {
                counter = 0;
                yield return null; // Wait for next frame
            }
        }

        TabInput[] keys = _fieldsToRulesRun.Keys.ToArray();
        foreach (var _sourceInput in keys)
        {
            _fieldsToRulesRun[_sourceInput] = false;
        }

        onComplete?.Invoke();
    }
}
