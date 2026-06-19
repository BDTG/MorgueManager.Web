using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MorgueManager.Web.Services
{
    public class TourStep
    {
        public string Selector { get; set; } = "";
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string Position { get; set; } = "bottom"; // bottom, top, left, right
    }

    public class TourService
    {
        private readonly NavigationManager _navigationManager;

        public TourService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public event Action? OnTourStateChanged;

        public bool IsActive { get; private set; }
        public bool IsSelectionModalVisible { get; private set; }
        public string CurrentModule { get; private set; } = "";
        public int CurrentStepIndex { get; private set; } = -1;
        public List<TourStep> Steps { get; private set; } = new();

        private static readonly Dictionary<string, List<TourStep>> ModuleTours = new()
        {
            {
                "Dashboard", new List<TourStep>
                {
                    new() { Selector = "#tour-kpi-grid", Title = "Chỉ số thống kê nhanh (KPI)", Content = "Xem nhanh số liệu về tổng thi thể, đang lưu kho, các cảnh báo khẩn cấp và trạng thái bàn giao.", Position = "bottom" },
                    new() { Selector = "#tour-chart-container", Title = "Biểu đồ tiếp nhận & bàn giao", Content = "Theo dõi xu hướng tiếp nhận và bàn giao thi thể theo tuần, trực quan hóa tiến độ công việc.", Position = "bottom" },
                    new() { Selector = "#tour-quick-links", Title = "Truy cập nhanh", Content = "Lối tắt để truy cập nhanh các tính năng quan trọng như Quản lý Thi thể, Ngăn lưu trữ, Ca trực và Báo cáo.", Position = "top" },
                    new() { Selector = "#tour-recent-activity", Title = "Hoạt động gần đây", Content = "Danh sách các sự kiện vừa diễn ra trong ngày như tiếp nhận mới, di chuyển ngăn hoặc bàn giao.", Position = "top" }
                }
            },
            {
                "Corpses", new List<TourStep>
                {
                    new() { Selector = "#tour-btn-add-corpse", Title = "Thêm thi thể mới", Content = "Click vào đây để nhập thông tin tiếp nhận thi thể mới vào hệ thống.", Position = "bottom" },
                    new() { Selector = "#tour-filter-bar", Title = "Bộ lọc & Tìm kiếm", Content = "Tìm kiếm nhanh theo tên, mã thi thể hoặc lọc theo trạng thái, giới tính, khoảng thời gian.", Position = "bottom" },
                    new() { Selector = "#tour-corpse-table", Title = "Danh sách thi thể", Content = "Bảng hiển thị thông tin chi tiết. Nhấp vào một dòng để xem chi tiết thông tin thân nhân, ngăn lưu trữ.", Position = "top" }
                }
            },
            {
                "Storages", new List<TourStep>
                {
                    new() { Selector = "#tour-storage-legend", Title = "Lọc/Legend Trạng thái", Content = "Chú giải màu sắc trạng thái ngăn tủ (Trống, Đang sử dụng, Chờ vệ sinh, Bảo trì) và số lượng cụ thể.", Position = "bottom" },
                    new() { Selector = "#tour-storage-zone-a", Title = "Sơ đồ chi tiết khu vực", Content = "Sơ đồ hiển thị vị trí các ngăn tủ theo từng Khu (A, B, C). Bạn có thể nhấp trực tiếp vào ngăn để xem chi tiết hoặc thực hiện thao tác nhanh.", Position = "top" }
                }
            },
            {
                "Temperature", new List<TourStep>
                {
                    new() { Selector = "#tour-temp-cards", Title = "Gauges nhiệt độ Khu vực", Content = "Giám sát nhiệt độ thời gian thực của từng khu vực kho lạnh. Bạn cũng có thể bấm 'Điều chỉnh' để thay đổi nhiệt độ mục tiêu.", Position = "bottom" },
                    new() { Selector = "#tour-temp-controls", Title = "Bảng điều khiển & Chi tiết ngăn tủ", Content = "Theo dõi nhiệt độ của từng ngăn tủ riêng biệt cùng đồ thị dao động nhiệt độ 30 phút qua.", Position = "top" }
                }
            },
            {
                "Shifts", new List<TourStep>
                {
                    new() { Selector = "#tour-btn-add-shift", Title = "Phân ca trực mới", Content = "Click vào đây để mở bảng phân công ca trực mới cho nhân viên trực ca.", Position = "bottom" },
                    new() { Selector = "#tour-calendar", Title = "Lịch trực tháng", Content = "Lịch hiển thị trực quan các ca trực trong tháng của nhân viên. Có cảnh báo nếu ca trực đó chưa có nhân viên đảm nhiệm.", Position = "top" }
                }
            }
        };

        public void ShowSelectionModal()
        {
            IsSelectionModalVisible = true;
            IsActive = false;
            NotifyStateChanged();
        }

        public void HideSelectionModal()
        {
            IsSelectionModalVisible = false;
            NotifyStateChanged();
        }

        public void StartTour(string moduleName)
        {
            IsSelectionModalVisible = false;
            if (ModuleTours.TryGetValue(moduleName, out var steps))
            {
                IsActive = true;
                CurrentModule = moduleName;
                CurrentStepIndex = 0;
                Steps = steps;
                
                string targetRoute = moduleName switch
                {
                    "Dashboard" => "/admin/dashboard",
                    "Corpses" => "/admin/corpses",
                    "Storages" => "/admin/storages",
                    "Temperature" => "/admin/temperature",
                    "Shifts" => "/admin/shifts",
                    _ => ""
                };

                NotifyStateChanged();

                if (!string.IsNullOrEmpty(targetRoute) && !_navigationManager.Uri.EndsWith(targetRoute))
                {
                    _navigationManager.NavigateTo(targetRoute);
                }
            }
        }

        public void NextStep()
        {
            if (!IsActive) return;
            if (CurrentStepIndex < Steps.Count - 1)
            {
                CurrentStepIndex++;
                NotifyStateChanged();
            }
            else
            {
                EndTour();
            }
        }

        public void PrevStep()
        {
            if (!IsActive) return;
            if (CurrentStepIndex > 0)
            {
                CurrentStepIndex--;
                NotifyStateChanged();
            }
        }

        public void EndTour()
        {
            IsActive = false;
            CurrentModule = "";
            CurrentStepIndex = -1;
            Steps.Clear();
            NotifyStateChanged();
        }

        public void TriggerStateChangeFromPage()
        {
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnTourStateChanged?.Invoke();
    }
}
