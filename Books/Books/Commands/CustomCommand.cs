using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System;

namespace Books.Commands
{
    public class CustomCommand : ICommand
    {
        private readonly Action<object?, EventArgs>? startEvent;
        private readonly Action<object?>? startWithParam;
        private readonly Action? startWithoutParam;
        private bool isCanExecute;
        public CustomCommand(Action<object?> startWithParam, bool canExecute = true)
        {
            this.startWithParam = startWithParam;
            isCanExecute = canExecute;
        }
        public CustomCommand(Action startWithoutParam, bool canExecute = true)
        {
            this.startWithoutParam = startWithoutParam;
            isCanExecute = canExecute;
        }
        public CustomCommand(Action<object?, EventArgs> startEvent)
        {
            this.startEvent = startEvent;
        }
        public bool IsCanExecute
        {
            get
            {
                return isCanExecute;
            }
            set
            {
                if (isCanExecute != value)
                {
                    isCanExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter)
        {
            return IsCanExecute;
        }
        public void Execute(object? parameter)
        {
            startWithoutParam?.Invoke();
            startWithParam?.Invoke(parameter);
        }
        public void ExecuteEvent(object o, EventArgs e)
        {
            startEvent?.Invoke(o, e);
        }
    }

    public class CustomEventCommand : DependencyObject
    {
        public string? EventName { get; set; }
        public string? CommandName { get; set; }
        public void Bind(object o)
        {
            if (!string.IsNullOrEmpty(EventName))
            {
                EventInfo? eventInfo = o.GetType().GetEvent(EventName);
                MethodInfo? methodInfo = GetType().GetMethod("EventProxy", BindingFlags.NonPublic | BindingFlags.Instance);
                if (eventInfo != null && methodInfo != null && eventInfo.EventHandlerType != null)
                {
                    Delegate del = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
                    eventInfo.RemoveEventHandler(o, del);
                    eventInfo.AddEventHandler(o, del);
                }
            }
        }
        private void EventProxy(object o, EventArgs e)
        {
            if (o is FrameworkElement f)
            {
                object? ctx = FindContext(f);
                if (ctx != null)
                {
                    if (!string.IsNullOrEmpty(CommandName))
                    {
                        var propInfo = ctx.GetType().GetProperty(CommandName);
                        if (propInfo != null)
                        {
                            var getter = propInfo.GetGetMethod();
                            if (getter != null)
                            {
                                var command = getter.Invoke(ctx, Array.Empty<object>()) as CustomCommand;
                                if (command != null)
                                {
                                    command.ExecuteEvent(o, e);
                                }
                            }
                        }
                    }
                }
            }
        }
        private static object? FindContext(FrameworkElement? f)
        {
            while (f != null)
            {
                if (f.DataContext != null) return f.DataContext;
                f = VisualTreeHelper.GetParent(f) as FrameworkElement;
            }
            return null;
        }
    }
    public class CustomEventCommandsCollection : ObservableCollection<CustomEventCommand>
    {
    }
    public static class EventBindings
    {
        private static readonly DependencyProperty EventBindingsProperty =
          DependencyProperty.RegisterAttached("EventBindings",
          typeof(CustomEventCommandsCollection), typeof(EventBindings),
          new PropertyMetadata(null, new PropertyChangedCallback(OnEventBindingsChanged)));
        public static CustomEventCommandsCollection GetEventBindings(DependencyObject o)
        {
            return (CustomEventCommandsCollection)o.GetValue(EventBindingsProperty);
        }
        public static void SetEventBindings(DependencyObject o, CustomEventCommandsCollection value)
        {
            o.SetValue(EventBindingsProperty, value);
        }
        public static void OnEventBindingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is CustomEventCommandsCollection newEventBindings)
            {
                foreach (CustomEventCommand binding in newEventBindings)
                {
                    binding.Bind(o);
                }
            }
        }
    }
}
