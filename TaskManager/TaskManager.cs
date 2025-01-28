using System.Diagnostics;

namespace WinFormsApp1
{
    public partial class TaskManager : Form
    {
        public ListView listView;
        private ImageList imageList;
        private ImageList icons;
        private Dictionary<int, ProcessInfo> currentProcesses;
        private ListViewItem lastInteractedItem;
        private ContextMenuStrip contextMenu;

        public TaskManager()
        {
            InitializeComponent();

            this.CenterToScreen();

            listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Font = new Font("Arial", 13),
                CheckBoxes = true,
            };

            // Add columns
            listView.Columns.Add("Process", 300);
            listView.Columns.Add("RAM", 150);
            listView.Columns.Add("StartTime", 230);
            listView.Columns.Add("Priority", 150);
            listView.Columns.Add("ThreadsNumber", 170);

            // Set up ImageList for icons
            icons = new ImageList();
            icons.ImageSize = new Size(32, 32);
            imageList = new ImageList();
            imageList.ImageSize = new Size(26, 26);
            imageList.Images.Add("empty", new Bitmap(1, 1));

            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string[] imagePathes = [
                Path.Combine(exePath, "expand.png"),
                Path.Combine(exePath, "expand_hover.png"),
                Path.Combine(exePath, "collapse.png"),
                Path.Combine(exePath, "collapse_hover.png"),
                ];

            imageList.Images.Add("expand", Image.FromFile(imagePathes[0]));
            imageList.Images.Add("expand_hover", Image.FromFile(imagePathes[1])); // Regular expand icon
            imageList.Images.Add("collapse", Image.FromFile(imagePathes[2])); // Highlighted expand icon
            imageList.Images.Add("collapse_hover", Image.FromFile(imagePathes[3])); // Regular collapse icon


            listView.SmallImageList = icons;
            listView.StateImageList = imageList;

            listView.MouseClick += OnListViewMouseClick;
            listView.MouseMove += OnListViewMouseMove;
            listView.ItemCheck += OnItemChecked;
            listView.MouseUp += OnListViewMouseUp;

            this.Controls.Add(listView);

            contextMenu = new ContextMenuStrip();
            var executeProcessMenuItem = new ToolStripMenuItem("Новий процес", null, ExecuteProcess);
            var endProcessMenuItem = new ToolStripMenuItem("Завершити процес", null, OnEndProcessClicked);
            var changePriorityMenuItem = new ToolStripMenuItem("Змінити пріоритет");

            // Масив пар тексту та відповідного пріоритету
            var priorityItems = new (string Text, ProcessPriorityClass Priority)[]
            {
                ("Реального часу", ProcessPriorityClass.RealTime),
                ("Високий", ProcessPriorityClass.High),
                ("Вище середнього", ProcessPriorityClass.AboveNormal),
                ("Середній", ProcessPriorityClass.Normal),
                ("Нижче середнього", ProcessPriorityClass.BelowNormal),
                ("Низький", ProcessPriorityClass.Idle)
            };

            // Створення пунктів меню динамічно
            foreach (var (text, priority) in priorityItems)
            {
                var menuItem = new ToolStripMenuItem(text, null, (sender, e) => OnChangePriorityClicked(sender, e, priority));
                changePriorityMenuItem.DropDownItems.Add(menuItem);
            }

            contextMenu.Items.Add(executeProcessMenuItem);
            contextMenu.Items.Add(endProcessMenuItem);
            contextMenu.Items.Add(changePriorityMenuItem);

            currentProcesses = new Dictionary<int, ProcessInfo>();

            Task.Run(() => PeriodicProcessUpdate());
        }
        private async void PeriodicProcessUpdate()
        {
            while (true)
            {
                await Task.Delay(1000);
                await Task.Run(() => LoadProcesses());
            }
        }
        private void LoadProcesses()
        {
            var processes = new List<ProcessInfo>();

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    var threads = new List<ThreadInfo>();
                    foreach (ProcessThread thread in process.Threads)
                    {
                        threads.Add(new ThreadInfo { Id = thread.Id, State = thread.ThreadState.ToString() });
                    }

                    Icon icon = null;
                    try
                    {
                        icon = Icon.ExtractAssociatedIcon(process.MainModule?.FileName ?? string.Empty);
                    }
                    catch
                    {
                        // Ignore icon extraction errors
                    }

                    processes.Add(new ProcessInfo
                    {
                        Name = process.ProcessName,
                        Id = process.Id,
                        Priority = process.PriorityClass.ToString(),
                        Threads = threads.ToArray(),
                        RamUsage = process.WorkingSet64,
                        StartTime = process.StartTime.ToString("G"),
                        Icon = icon
                    });
                }
                catch
                {
                    // Ignore processes that cannot be accessed
                }
            }

            Invoke(new MethodInvoker(() => UpdateListView(processes)));
        }
        private void UpdateListView(List<ProcessInfo> newProcesses)
        {
            listView.BeginUpdate();

            var newProcessesDict = newProcesses.ToDictionary(p => p.Id);

            // Update existing items or add new ones
            foreach (var process in newProcesses)
            {
                if (currentProcesses.TryGetValue(process.Id, out var existingProcess))
                {
                    // Update if different
                    var item = listView.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Tag is ProcessInfo pi && pi.Id == process.Id);
                    if (item != null)
                    {
                        item.SubItems[1].Text = (process.RamUsage / 1024 / 1024).ToString("N0") + " MB";
                        item.SubItems[2].Text = process.StartTime;
                        item.SubItems[3].Text = process.Priority.ToString();
                        item.SubItems[4].Text = process.Threads.Length.ToString();
                    }
                }
                else
                {
                    // Add new process
                    var item = new ListViewItem(process.Name)
                    {
                        Tag = process,
                        StateImageIndex = 1,
                    };
                    item.SubItems.Add((process.RamUsage / 1024 / 1024).ToString("N0") + " MB");
                    item.SubItems.Add(process.StartTime);
                    item.SubItems.Add(process.Priority.ToString());
                    item.SubItems.Add(process.Threads.Length.ToString());

                    if (process.Icon != null)
                    {
                        icons.Images.Add(process.Id.ToString(), process.Icon);
                        item.ImageKey = process.Id.ToString();
                    }

                    listView.Items.Add(item);
                }
            }

            // Remove processes that are no longer running
            foreach (ListViewItem item in listView.Items.Cast<ListViewItem>().ToList())
            {
                if (item.Tag is ProcessInfo pi && !newProcessesDict.ContainsKey(pi.Id))
                {
                    listView.Items.Remove(item);
                }
            }

            currentProcesses = newProcessesDict;

            listView.EndUpdate();
        }
        private void OnListViewMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = listView.HitTest(e.Location);
                if (hitTest.Item != null)
                {
                    listView.SelectedItems.Clear();
                    hitTest.Item.Selected = true;
                    contextMenu.Show(listView, e.Location);
                }
            }
        }
        private void OnItemChecked(object sender, ItemCheckEventArgs e)
        {
            var item = listView.Items[e.Index];
            if (item.Tag is false)
            {
                e.NewValue = e.CurrentValue;
                item.StateImageIndex = 0;
            }
        }
        private void OnListViewMouseClick(object sender, MouseEventArgs e)
        {
            var hitTest = listView.HitTest(e.Location);
            if (hitTest.Item != null)
            {
                var selectedItem = hitTest.Item;

                int checkboxWidth = listView.StateImageList.ImageSize.Width;
                int checkboxOffset = 4; // Adjust if needed
                Rectangle checkboxBounds = new Rectangle(
                    selectedItem.Bounds.Left + checkboxOffset,
                    selectedItem.Bounds.Top + (selectedItem.Bounds.Height - checkboxWidth) / 2,
                    checkboxWidth,
                    checkboxWidth
                );

                // Check if the mouse click is within the icon bounds
                if (checkboxBounds.Contains(e.Location))
                {
                    // Check the current state of the icon
                    if (selectedItem.StateImageIndex == 2)
                    {
                        ExpandSubitems(selectedItem);
                        selectedItem.StateImageIndex = 3; // Change icon to collapse
                    }
                    else if (selectedItem.StateImageIndex == 4)
                    {
                        CollapseSubitems(selectedItem);
                        selectedItem.StateImageIndex = 1; // Change icon to expand
                    }
                }
            }
        }
        private void OnListViewMouseMove(object sender, MouseEventArgs e)
        {
            var hitTest = listView.HitTest(e.Location);
            if (hitTest.Item != null)
            {
                var selectedItem = hitTest.Item;

                if (lastInteractedItem != selectedItem)
                {
                    if (lastInteractedItem?.StateImageIndex == 2)
                    {
                        lastInteractedItem.StateImageIndex = 1;
                    }
                    if (lastInteractedItem?.StateImageIndex == 4)
                    {
                        lastInteractedItem.StateImageIndex = 3;
                    }
                }

                lastInteractedItem = hitTest.Item;

                if (!selectedItem.Checked)
                {
                    selectedItem.StateImageIndex = 0;
                }

                int checkboxWidth = listView.StateImageList.ImageSize.Width;
                int checkboxOffset = 4; // Adjust if needed
                Rectangle checkboxBounds = new Rectangle(
                    selectedItem.Bounds.Left + checkboxOffset,
                    selectedItem.Bounds.Top + (selectedItem.Bounds.Height - checkboxWidth) / 2,
                    checkboxWidth,
                    checkboxWidth
                );

                // Check if the mouse is over the checkbox
                if (checkboxBounds.Contains(e.Location))
                {
                    // Change the state image to hover version
                    if (selectedItem.StateImageIndex == 1)
                    {
                        selectedItem.StateImageIndex = 2; // Index for "checked_hover" image
                    }
                    else if (selectedItem.StateImageIndex == 3)
                    {
                        selectedItem.StateImageIndex = 4; // Index for "unchecked_hover" image
                    }
                    return;
                }
            }
        }
        private void ExpandSubitems(ListViewItem parentItem)
        {
            if (parentItem.Tag is ProcessInfo processInfo)
            {
                int index = parentItem.Index + 1;
                listView.BeginUpdate();

                foreach (var thread in processInfo.Threads)
                {
                    var subItem = new ListViewItem("  Thread ID: " + thread.Id)
                    {
                        ForeColor = Color.Gray,
                        Tag = thread
                    };
                    subItem.SubItems.Add("");
                    subItem.SubItems.Add("");
                    subItem.SubItems.Add(thread.State);

                    listView.Items.Insert(index++, subItem);
                }

                listView.EndUpdate();
            }
        }
        private void CollapseSubitems(ListViewItem parentItem)
        {
            if (parentItem.Tag is ProcessInfo processInfo)
            {
                int index = parentItem.Index + 1;
                listView.BeginUpdate();

                foreach (var _ in processInfo.Threads)
                {
                    listView.Items.RemoveAt(index);
                }

                listView.EndUpdate();
            }
        }
        private void OnEndProcessClicked(object sender, EventArgs e)
        {
            try
            {
                if (listView.FocusedItem != null)
                {
                    string processName = listView.FocusedItem.SubItems[0].Text;
                    var processes = Process.GetProcessesByName(processName);
                    foreach (var process in processes)
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося завершити процес: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OnChangePriorityClicked(object sender, EventArgs e, ProcessPriorityClass priority)
        {
            try
            {
                if (listView.FocusedItem != null)
                {
                    string processName = listView.FocusedItem.SubItems[0].Text;

                    var processes = Process.GetProcessesByName(processName);
                    foreach (var process in processes)
                    {
                        switch (priority)
                        {
                            case ProcessPriorityClass.RealTime:
                                process.PriorityClass = ProcessPriorityClass.RealTime;
                                break;
                            case ProcessPriorityClass.High:
                                process.PriorityClass = ProcessPriorityClass.High;
                                break;
                            case ProcessPriorityClass.AboveNormal:
                                process.PriorityClass = ProcessPriorityClass.AboveNormal;
                                break;
                            case ProcessPriorityClass.Normal:
                                process.PriorityClass = ProcessPriorityClass.Normal;
                                break;
                            case ProcessPriorityClass.BelowNormal:
                                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                                break;
                            case ProcessPriorityClass.Idle:
                                process.PriorityClass = ProcessPriorityClass.Idle;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося змінити пріоритет: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ExecuteProcess(object sender, EventArgs e)
        {
            using (var inputDialog = new InputDialog("Запустити процес"))
            {
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    string userInput = inputDialog.UserInput;

                    // Список відомих програм і шляхів до них
                    var knownPrograms = new Dictionary<string, string>
                {
                    { "winword.exe", @"D:\Tools\Office\OFFICE\root\Office16\WINWORD.EXE" },
                    { "excel.exe", @"D:\Tools\Office\OFFICE\root\Office16\EXCEL.EXE" },
                    // Додайте інші програми, якщо потрібно
                };

                    try
                    {
                        if (knownPrograms.ContainsKey(userInput.ToLower()))
                        {
                            Process.Start(knownPrograms[userInput.ToLower()]);
                        }
                        else
                        { 
                            Process.Start(userInput);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Не вдалось запустити процес");
                    }
                }
            }
        }
    }

    public class ProcessInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Priority { get; set; }
        public ThreadInfo[] Threads { get; set; }
        public long RamUsage { get; set; }
        public string StartTime { get; set; }
        public Icon Icon { get; set; }
    }
    public class ThreadInfo
    {
        public int Id { get; set; }
        public string State { get; set; }
    }
}
