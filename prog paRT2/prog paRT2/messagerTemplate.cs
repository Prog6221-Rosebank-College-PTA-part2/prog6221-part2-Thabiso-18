using System.Windows;
using System.Windows.Controls;

namespace CybersecurityBotWPF
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (item is ChatMessage message)
            {
                if (message.IsUser)
                {
                    return element.FindResource("UserMessageTemplate") as DataTemplate;
                }
                else
                {
                    return element.FindResource("BotMessageTemplate") as DataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}