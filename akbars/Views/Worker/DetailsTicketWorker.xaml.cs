using System;
using System.Linq;
using System.Windows;
using akbars.Models;
using akbars.Services;

namespace akbars.Views.Worker
{
    public partial class DetailsTicketWorker : Window
    {
        private readonly SessionContext _session;
        private TicketDetails _details;
        private readonly bool _allowActions;

        public DetailsTicketWorker(SessionContext session, TicketDetails details, bool allowActions)
        {
            InitializeComponent();
            _session = session;
            _details = details;
            _allowActions = allowActions;
            Render();
        }

        private void Render()
        {
            if (_details == null)
            {
                return;
            }

            WindowTitleText.Text = "Заявка #" + _details.Id;
            WindowSubtitleText.Text = "Полная карточка заявки с историей изменений и контекстом исполнения.";
            TicketIdText.Text = _details.Id.ToString();
            StatusText.Text = _details.StatusName;
            PriorityText.Text = _details.PriorityName;
            TypeText.Text = _details.TypeName;
            AuthorText.Text = string.IsNullOrWhiteSpace(_details.AuthorName) ? "Не указан" : _details.AuthorName;
            ExecutorText.Text = string.IsNullOrWhiteSpace(_details.AssigneeName) ? "Не назначен" : _details.AssigneeName;
            ShortDescriptionText.Text = _details.ShortDescription;
            DetailedDescriptionText.Text = string.IsNullOrWhiteSpace(_details.DetailedDescription) ? "Подробности не указаны." : _details.DetailedDescription;
            CreatedAtText.Text = _details.CreatedAt.ToString("dd.MM.yyyy HH:mm");
            UpdatedAtText.Text = _details.UpdatedAt.ToString("dd.MM.yyyy HH:mm");
            CompletedAtText.Text = _details.CompletedAt.HasValue
                ? "Завершена: " + _details.CompletedAt.Value.ToString("dd.MM.yyyy HH:mm")
                : "Завершение ещё не зафиксировано.";

            HistoryList.ItemsSource = _details.History.Count == 0
                ? new[] { "История изменений не найдена или таблица истории отсутствует в схеме." }
                : _details.History.Select(item =>
                    string.Format(
                        "{0:dd.MM.yyyy HH:mm} • {1}: {2} → {3}",
                        item.ChangedAt,
                        item.FieldName,
                        string.IsNullOrWhiteSpace(item.OldValue) ? "—" : item.OldValue,
                        string.IsNullOrWhiteSpace(item.NewValue) ? "—" : item.NewValue));

            var actionVisibility = _allowActions ? Visibility.Visible : Visibility.Collapsed;
            StartButton.Visibility = actionVisibility;
            CompleteButton.Visibility = actionVisibility;
            CancelButton.Visibility = actionVisibility;
            ActionNoteBox.Visibility = actionVisibility;
            ActionHintText.Visibility = actionVisibility;

            if (!_allowActions)
            {
                ActionHintText.Text = "В этом режиме карточка доступна только для просмотра.";
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus("start");
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus("complete");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ChangeStatus("cancel");
        }

        private void ChangeStatus(string mode)
        {
            try
            {
                switch (mode)
                {
                    case "start":
                        AppServices.TicketService.StartTicket(_details.Id, _session.UserId, ActionNoteBox.Text);
                        break;
                    case "complete":
                        AppServices.TicketService.CompleteTicket(_details.Id, _session.UserId, ActionNoteBox.Text);
                        break;
                    case "cancel":
                        AppServices.TicketService.CancelTicket(_details.Id, _session.UserId, ActionNoteBox.Text);
                        break;
                }

                _details = AppServices.TicketService.GetTicketDetails(_details.Id);
                Render();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось обновить статус: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
