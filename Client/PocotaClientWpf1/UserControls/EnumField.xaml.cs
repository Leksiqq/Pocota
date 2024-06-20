﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Net.Leksi.Pocota.Client.UserControls;
public partial class EnumField : UserControl, ICommand, IFieldOwner
{
    public static readonly DependencyProperty FieldProperty = DependencyProperty.Register(
       nameof(Field), typeof(IField),
       typeof(EnumField)
    );
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
       nameof(Target), typeof(object),
       typeof(EnumField)
    );
    public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
       nameof(PropertyName), typeof(string),
       typeof(EnumField)
    );
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }
    private IField.WaitingForFlags _waitingFor = IField.WaitingForFlags.Any;
    public IField? Field
    {
        get => (IField?)GetValue(FieldProperty);
        set => SetValue(FieldProperty, value);
    }
    public object? Target
    {
        get => (object?)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }
    public string? PropertyName
    {
        get => (string?)GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }
    public List<object?> Items { get; private init; } = [];
    public EnumField()
    {
        InitializeComponent();
    }
    public bool CanExecute(object? parameter)
    {
        bool res = Field?.IsReady ?? false
        && (
            "Undo".Equals(parameter)
            || ("Clear".Equals(parameter) && !Field.IsClean)
        );
        return res;
    }
    public void Execute(object? parameter)
    {
        if (
            Field?.IsReady ?? false
            && (
                "Undo".Equals(parameter)
                || ("Clear".Equals(parameter) && !Field.IsClean)
            )
        )
        {
            if ("Undo".Equals(parameter))
            {
                //TODO Execute
            }
            else if ("Clear".Equals(parameter))
            {
                Field.Clear();
            }
        }
    }
    public void OnFieldAssigned()
    {
        if(Field is { })
        {
            ComboBox.DataContext = Field;
            UndoButton.Visibility = Field.EntityProperty?.Entity.State is EntityState.Unchanged || Field.EntityProperty?.Entity.State is EntityState.Modified
                ? Visibility.Visible : Visibility.Collapsed;
            if (Field.Type.IsEnum)
            {
                foreach (object item in Enum.GetValues(Field.Type))
                {
                    Items.Add(item);
                }
            }
            else
            {
                Items.Add(true);
                Items.Add(false);
            }
        }
    }
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == FieldProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.Field))
            {
                if (e.NewValue is IField newField)
                {
                    newField.Owner = this;
                }
            }
        }
        else if (e.Property == PropertyNameProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.PropertyName))
            {
                if (_waitingFor is IField.WaitingForFlags.None)
                {
                    Field = new Field { Target = Target, PropertyName = PropertyName, Owner = this };
                }
            }
        }
        else if (e.Property == TargetProperty)
        {
            if (IField.CanProcessProperty(_waitingFor, IField.WaitingForFlags.Target))
            {
                if (_waitingFor is IField.WaitingForFlags.None)
                {
                    Field = new Field { Target = Target, PropertyName = PropertyName, Owner = this };
                }
            }
        }
        base.OnPropertyChanged(e);
    }
}
