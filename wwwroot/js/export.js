window.ExportHelper = {
    loadScript: function (src) {
        return new Promise(function (resolve, reject) {
            if (document.querySelector('script[src="' + src + '"]')) {
                resolve();
                return;
            }
            var s = document.createElement('script');
            s.src = src;
            s.onload = resolve;
            s.onerror = reject;
            document.head.appendChild(s);
        });
    },

    initExportLibraries: async function () {
        try {
            if (typeof XLSX === 'undefined') {
                await this.loadScript('https://cdn.jsdelivr.net/npm/xlsx@0.18.5/dist/xlsx.full.min.js');
            }
            if (typeof window.jspdf === 'undefined') {
                await this.loadScript('https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js');
            }
            if (typeof window.jspdf !== 'undefined' && typeof window.jspdf.jsPDF !== 'undefined') {
                await this.loadScript('https://cdnjs.cloudflare.com/ajax/libs/jspdf-autotable/3.5.31/jspdf.plugin.autotable.min.js');
            }
            return true;
        } catch (e) {
            console.error('Error loading export libraries:', e);
            return false;
        }
    },

    loadFonts: async function (doc) {
        try {
            // Using Cloudflare cdnjs URLs (which return 200 OK) instead of jsDelivr (which returns 403 Forbidden)
            var fontUrl = 'https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.66/fonts/Roboto/Roboto-Regular.ttf';
            var fontRes = await fetch(fontUrl);
            if (!fontRes.ok) throw new Error('CDN regular font loading failed');
            var fontBuffer = await fontRes.arrayBuffer();

            var binary = '';
            var bytes = new Uint8Array(fontBuffer);
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            var base64Font = window.btoa(binary);

            doc.addFileToVFS('Roboto-Regular.ttf', base64Font);
            doc.addFont('Roboto-Regular.ttf', 'Roboto', 'normal');

            var boldUrl = 'https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.66/fonts/Roboto/Roboto-Medium.ttf';
            var boldRes = await fetch(boldUrl);
            if (boldRes.ok) {
                var boldBuffer = await boldRes.arrayBuffer();
                var boldBinary = '';
                var boldBytes = new Uint8Array(boldBuffer);
                for (var j = 0; j < boldBytes.byteLength; j++) {
                    boldBinary += String.fromCharCode(boldBytes[j]);
                }
                var base64Bold = window.btoa(boldBinary);
                doc.addFileToVFS('Roboto-Bold.ttf', base64Bold);
                doc.addFont('Roboto-Bold.ttf', 'Roboto', 'bold');
            }
        } catch (e) {
            console.error('Failed to load Unicode fonts, fallback to standard.', e);
        }
    },

    exportToExcel: function (dataJson) {
        var rawData = JSON.parse(dataJson);
        var data = rawData.map(function (item) {
            return {
                'Mã Ca': item.CaseId,
                'Họ Tên': item.Name,
                'Số CCCD': item.Cccd,
                'Giới Tính': item.Gender,
                'Tuổi': item.Age,
                'Nguyên Nhân Tử Vong': item.CauseOfDeath,
                'Trạng Thái': item.Status,
                'Khu Vực': item.StorageUnit,
                'Ngăn Tủ': item.StorageSlot,
                'Nhiệt Độ': item.Temp,
                'Số Ngày Lưu Kho': item.DaysStored,
                'Ngày Tiếp Nhận': item.DateAdmitted,
                'Ngày Tử Vong': item.DateOfDeath,
                'Tên Thân Nhân': item.NokName,
                'SĐT Thân Nhân': item.NokPhone,
                'Quan Hệ': item.NokRelationship,
                'Ghi Chú': item.Notes
            };
        });

        var ws = XLSX.utils.json_to_sheet(data);
        var wscols = [
            { wch: 15 }, { wch: 22 }, { wch: 15 }, { wch: 10 }, { wch: 8 },
            { wch: 25 }, { wch: 12 }, { wch: 15 }, { wch: 10 }, { wch: 10 },
            { wch: 15 }, { wch: 15 }, { wch: 15 }, { wch: 20 }, { wch: 15 },
            { wch: 12 }, { wch: 25 }
        ];
        ws['!cols'] = wscols;

        var wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Danh sách thi thể');

        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        XLSX.writeFile(wb, 'Danh_sach_thi_the_' + dateStr + '.xlsx');
    },

    exportToPdf: async function (dataJson) {
        var rawData = JSON.parse(dataJson);
        var { jsPDF } = window.jspdf;

        var doc = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' });

        await this.loadFonts(doc);

        doc.setFont('Roboto', 'bold');
        doc.setFontSize(18);
        doc.setTextColor(30, 41, 59);
        doc.text('HỆ THỐNG QUẢN LÝ NHÀ XÁC — MORGUEMANAGER', 14, 18);

        doc.setFont('Roboto', 'normal');
        doc.setFontSize(11);
        doc.setTextColor(100, 116, 139);
        doc.text('BÁO CÁO DANH SÁCH THI THỂ ĐANG QUẢN LÝ', 14, 24);

        var now = new Date();
        var formattedDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' ' + now.getHours().toString().padStart(2, '0') + ':' + now.getMinutes().toString().padStart(2, '0');
        doc.text('Ngày xuất báo cáo: ' + formattedDate, 220, 24);

        doc.setDrawColor(226, 232, 240);
        doc.setLineWidth(0.5);
        doc.line(14, 28, 283, 28);

        var columns = [
            { header: 'MÃ CA', dataKey: 'CaseId' },
            { header: 'HỌ VÀ TÊN', dataKey: 'Name' },
            { header: 'GIỚI TÍNH', dataKey: 'Gender' },
            { header: 'TUỔI', dataKey: 'Age' },
            { header: 'NGUYÊN NHÂN TỬ VONG', dataKey: 'CauseOfDeath' },
            { header: 'TRẠNG THÁI', dataKey: 'Status' },
            { header: 'VỊ TRÍ LƯU TRỮ', dataKey: 'Storage' },
            { header: 'NHIỆT ĐỘ', dataKey: 'Temp' },
            { header: 'NGÀY TIẾP NHẬN', dataKey: 'DateAdmitted' }
        ];

        doc.autoTable({
            columns: columns,
            body: rawData,
            startY: 32,
            theme: 'striped',
            styles: { font: 'Roboto', fontSize: 9, cellPadding: 3, valign: 'middle' },
            headStyles: { fillColor: [15, 23, 42], textColor: [255, 255, 255], fontStyle: 'bold', fontSize: 9 },
            alternateRowStyles: { fillColor: [248, 250, 252] },
            columnStyles: {
                CaseId: { cellWidth: 28, fontStyle: 'bold' },
                Name: { cellWidth: 38 },
                Gender: { cellWidth: 20 },
                Age: { cellWidth: 16, halign: 'center' }, // Increased from 12 to 16 to prevent Vietnamese wrapping issue on "TUỔI"
                CauseOfDeath: { cellWidth: 45 },
                Status: { cellWidth: 22, halign: 'center' },
                Storage: { cellWidth: 45 },
                Temp: { cellWidth: 18, halign: 'center' },
                DateAdmitted: { cellWidth: 32 }
            },
            didDrawPage: function (data) {
                var pageCount = doc.internal.getNumberOfPages();
                doc.setFont('Roboto', 'normal');
                doc.setFontSize(8);
                doc.setTextColor(148, 163, 184);
                doc.text('Trang ' + data.pageNumber + ' / ' + pageCount, 260, 200);
                doc.text('Tài liệu bảo mật nội bộ — MorgueManager System', 14, 200);
            }
        });

        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        doc.save('Bao_cao_danh_sach_thi_the_' + dateStr + '.pdf');
    },

    exportStorageSlotsToExcel: function (dataJson) {
        var rawData = JSON.parse(dataJson);
        var data = rawData.map(function (item) {
            return {
                'Mã Ngăn': item.Id,
                'Khu Vực': item.Zone,
                'Trạng Thái': item.Status,
                'Nhiệt Độ': item.Temp,
                'Thi Thể Đang Lưu': item.CorpseName,
                'Số Ngày Đã Lưu': item.DaysStored,
                'Cập Nhật Cuối': item.LastUpdate
            };
        });
        var ws = XLSX.utils.json_to_sheet(data);
        var wscols = [
            { wch: 12 }, { wch: 12 }, { wch: 15 }, { wch: 12 }, { wch: 35 }, { wch: 15 }, { wch: 18 }
        ];
        ws['!cols'] = wscols;
        var wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Danh sách ngăn tủ');
        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        XLSX.writeFile(wb, 'Bao_cao_ngan_tu_' + dateStr + '.xlsx');
    },

    exportStorageSlotsToPdf: async function (dataJson) {
        var rawData = JSON.parse(dataJson);
        var { jsPDF } = window.jspdf;
        var doc = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' });

        await this.loadFonts(doc);

        doc.setFont('Roboto', 'bold');
        doc.setFontSize(18);
        doc.setTextColor(30, 41, 59);
        doc.text('HỆ THỐNG QUẢN LÝ NHÀ XÁC — MORGUEMANAGER', 14, 18);
        doc.setFont('Roboto', 'normal');
        doc.setFontSize(11);
        doc.setTextColor(100, 116, 139);
        doc.text('BÁO CÁO DANH SÁCH NGĂN TỦ LẠNH VÀ NHIỆT ĐỘ', 14, 24);
        var now = new Date();
        var formattedDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' ' + now.getHours().toString().padStart(2, '0') + ':' + now.getMinutes().toString().padStart(2, '0');
        doc.text('Ngày xuất báo cáo: ' + formattedDate, 220, 24);
        doc.setDrawColor(226, 232, 240);
        doc.line(14, 28, 283, 28);
        var columns = [
            { header: 'MÃ NGĂN', dataKey: 'Id' },
            { header: 'KHU VỰC', dataKey: 'Zone' },
            { header: 'TRẠNG THÁI', dataKey: 'Status' },
            { header: 'NHIỆT ĐỘ', dataKey: 'Temp' },
            { header: 'THI THỂ LƯU TRỮ', dataKey: 'CorpseName' },
            { header: 'THỜI GIAN LƯU', dataKey: 'DaysStored' },
            { header: 'CẬP NHẬT CUỐI', dataKey: 'LastUpdate' }
        ];
        doc.autoTable({
            columns: columns,
            body: rawData,
            startY: 32,
            theme: 'striped',
            styles: { font: 'Roboto', fontSize: 9, cellPadding: 3, valign: 'middle' },
            headStyles: { fillColor: [15, 23, 42], textColor: [255, 255, 255], fontStyle: 'bold' },
            alternateRowStyles: { fillColor: [248, 250, 252] },
            columnStyles: {
                Id: { cellWidth: 25, fontStyle: 'bold' },
                Zone: { cellWidth: 20 },
                Status: { cellWidth: 30 },
                Temp: { cellWidth: 25, halign: 'center' },
                CorpseName: { cellWidth: 100 },
                DaysStored: { cellWidth: 35, halign: 'center' },
                LastUpdate: { cellWidth: 35 }
            },
            didDrawPage: function (data) {
                var pageCount = doc.internal.getNumberOfPages();
                doc.setFont('Roboto', 'normal');
                doc.setFontSize(8);
                doc.setTextColor(148, 163, 184);
                doc.text('Trang ' + data.pageNumber + ' / ' + pageCount, 260, 200);
                doc.text('Tài liệu bảo mật nội bộ — MorgueManager System', 14, 200);
            }
        });
        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        doc.save('Bao_cao_ngan_tu_' + dateStr + '.pdf');
    },

    exportShiftsToExcel: function (dataJson) {
        var rawData = JSON.parse(dataJson);
        var data = rawData.map(function (item) {
            return {
                'Ngày': item.Date,
                'Ca Trực': item.Type,
                'Nhân Viên': item.Staff,
                'SĐT Liên Hệ': item.Phone,
                'Trạng Thái': item.Status,
                'Ghi Chú': item.Notes
            };
        });
        var ws = XLSX.utils.json_to_sheet(data);
        var wscols = [
            { wch: 12 }, { wch: 12 }, { wch: 35 }, { wch: 25 }, { wch: 18 }, { wch: 30 }
        ];
        ws['!cols'] = wscols;
        var wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Lịch ca trực');
        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        XLSX.writeFile(wb, 'Lich_ca_truc_' + dateStr + '.xlsx');
    },

    exportShiftsToPdf: async function (dataJson) {
        var rawData = JSON.parse(dataJson);
        var { jsPDF } = window.jspdf;
        var doc = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' });

        await this.loadFonts(doc);

        doc.setFont('Roboto', 'bold');
        doc.setFontSize(18);
        doc.setTextColor(30, 41, 59);
        doc.text('HỆ THỐNG QUẢN LÝ NHÀ XÁC — MORGUEMANAGER', 14, 18);
        doc.setFont('Roboto', 'normal');
        doc.setFontSize(11);
        doc.setTextColor(100, 116, 139);
        doc.text('BÁO CÁO PHÂN CÔNG LỊCH TRỰC NHÂN SỰ', 14, 24);
        var now = new Date();
        var formattedDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' ' + now.getHours().toString().padStart(2, '0') + ':' + now.getMinutes().toString().padStart(2, '0');
        doc.text('Ngày xuất báo cáo: ' + formattedDate, 220, 24);
        doc.setDrawColor(226, 232, 240);
        doc.line(14, 28, 283, 28);
        var columns = [
            { header: 'NGÀY', dataKey: 'Date' },
            { header: 'CA TRỰC', dataKey: 'Type' },
            { header: 'NHÂN VIÊN TRỰC', dataKey: 'Staff' },
            { header: 'SĐT LIÊN HỆ', dataKey: 'Phone' },
            { header: 'TRẠNG THÁI', dataKey: 'Status' },
            { header: 'GHI CHÚ', dataKey: 'Notes' }
        ];
        doc.autoTable({
            columns: columns,
            body: rawData,
            startY: 32,
            theme: 'striped',
            styles: { font: 'Roboto', fontSize: 9, cellPadding: 3, valign: 'middle' },
            headStyles: { fillColor: [15, 23, 42], textColor: [255, 255, 255], fontStyle: 'bold' },
            alternateRowStyles: { fillColor: [248, 250, 252] },
            columnStyles: {
                Date: { cellWidth: 25, fontStyle: 'bold' },
                Type: { cellWidth: 25 },
                Staff: { cellWidth: 80 },
                Phone: { cellWidth: 45 },
                Status: { cellWidth: 35 },
                Notes: { cellWidth: 60 }
            },
            didDrawPage: function (data) {
                var pageCount = doc.internal.getNumberOfPages();
                doc.setFont('Roboto', 'normal');
                doc.setFontSize(8);
                doc.setTextColor(148, 163, 184);
                doc.text('Trang ' + data.pageNumber + ' / ' + pageCount, 260, 200);
                doc.text('Tài liệu bảo mật nội bộ — MorgueManager System', 14, 200);
            }
        });
        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        doc.save('Bao_cao_lich_truc_' + dateStr + '.pdf');
    },

    exportSummaryToExcel: function (dataJson) {
        var payload = JSON.parse(dataJson);
        var wb = XLSX.utils.book_new();
        var statData = [
            { 'Chỉ số thống kê': 'Tổng số thi hài', 'Giá trị': payload.Stats.TotalCorpses },
            { 'Chỉ số thống kê': 'Thi hài đang lưu kho', 'Giá trị': payload.Stats.ActiveCorpses },
            { 'Chỉ số thống kê': 'Tổng số ngăn tủ', 'Giá trị': payload.Stats.TotalSlots },
            { 'Chỉ số thống kê': 'Ngăn tủ đang sử dụng', 'Giá trị': payload.Stats.OccupiedSlots },
            { 'Chỉ số thống kê': 'Tỷ lệ lấp đầy (%)', 'Giá trị': payload.Stats.OccupancyRate }
        ];
        var wsStats = XLSX.utils.json_to_sheet(statData);
        XLSX.utils.book_append_sheet(wb, wsStats, 'Tổng quan');
        var wsCorpses = XLSX.utils.json_to_sheet(payload.Corpses.map(function (item) {
            return {
                'Mã Ca': item.CaseId, 'Họ Tên': item.Name, 'Giới Tính': item.Gender, 'Tuổi': item.Age,
                'Nguyên Nhân': item.CauseOfDeath, 'Trạng Thái': item.Status, 'Ngăn tủ': item.Storage,
                'Nhiệt độ': item.Temp, 'Ngày nhập': item.DateAdmitted
            };
        }));
        XLSX.utils.book_append_sheet(wb, wsCorpses, 'Danh sách thi hài');
        var wsSlots = XLSX.utils.json_to_sheet(payload.Slots.map(function (item) {
            return {
                'Mã Ngăn': item.Id, 'Phân khu': item.Zone, 'Trạng Thái': item.Status,
                'Nhiệt Độ': item.Temp, 'Thi hài đang lưu': item.CorpseName, 'Số ngày lưu': item.DaysStored
            };
        }));
        XLSX.utils.book_append_sheet(wb, wsSlots, 'Sơ đồ ngăn tủ');
        var wsShifts = XLSX.utils.json_to_sheet(payload.Shifts.map(function (item) {
            return {
                'Ngày': item.Date, 'Ca trực': item.Type, 'Nhân viên': item.Staff,
                'Trạng Thái': item.Status, 'Ghi Chú': item.Notes
            };
        }));
        XLSX.utils.book_append_sheet(wb, wsShifts, 'Lịch ca trực');
        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        XLSX.writeFile(wb, 'Bao_cao_tong_hop_' + dateStr + '.xlsx');
    },

    exportSummaryToPdf: async function (dataJson) {
        var raw = JSON.parse(dataJson);
        var { jsPDF } = window.jspdf;
        var doc = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });

        await this.loadFonts(doc);

        doc.setFont('Roboto', 'bold');
        doc.setFontSize(18);
        doc.setTextColor(30, 41, 59);
        doc.text('HỆ THỐNG QUẢN LÝ NHÀ XÁC — MORGUEMANAGER', 14, 18);
        doc.setFont('Roboto', 'normal');
        doc.setFontSize(11);
        doc.setTextColor(100, 116, 139);
        doc.text('BÁO CÁO TỔNG HỢP HOẠT ĐỘNG TOÀN HỆ THỐNG', 14, 24);
        var now = new Date();
        var formattedDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear() + ' ' + now.getHours().toString().padStart(2, '0') + ':' + now.getMinutes().toString().padStart(2, '0');
        doc.text('Ngày xuất báo cáo: ' + formattedDate, 140, 24);
        doc.setDrawColor(226, 232, 240);
        doc.line(14, 28, 196, 28);
        doc.setFont('Roboto', 'bold');
        doc.setFontSize(14);
        doc.setTextColor(15, 23, 42);
        doc.text('I. THỐNG KÊ CHỈ SỐ HOẠT ĐỘNG', 14, 38);
        doc.setFont('Roboto', 'normal');
        doc.setFontSize(10);
        doc.setTextColor(51, 65, 85);
        var startY = 44;
        doc.text('• Tổng số hồ sơ thi hài hệ thống: ' + raw.TotalCorpses, 18, startY);
        doc.text('• Số thi hài hiện đang lưu kho: ' + raw.ActiveCorpses, 18, startY + 6);
        doc.text('• Tỷ lệ lấp đầy hộc lạnh: ' + raw.SlotStats.OccupancyRate + '%', 18, startY + 12);
        doc.text('• Tổng số hộc lạnh (tủ mát & cấp đông): ' + raw.SlotStats.Total, 110, startY);
        doc.text('• Đang sử dụng (Occupied): ' + raw.SlotStats.Occupied + ' hộc', 110, startY + 6);
        doc.text('• Trống / Đang vệ sinh / Bảo trì: ' + raw.SlotStats.Empty + ' / ' + raw.SlotStats.Cleaning + ' / ' + raw.SlotStats.Maint, 110, startY + 12);
        doc.setFont('Roboto', 'bold');
        doc.setFontSize(14);
        doc.text('II. DANH SÁCH THI HÀI LƯU KHO GẦN ĐÂY', 14, 70);
        var columns = [
            { header: 'MÃ CA', dataKey: 'CaseId' },
            { header: 'HỌ VÀ TÊN', dataKey: 'Name' },
            { header: 'TRẠNG THÁI', dataKey: 'Status' },
            { header: 'NGĂN TỦ', dataKey: 'Storage' },
            { header: 'NGÀY TIẾP NHẬN', dataKey: 'DateAdmitted' }
        ];
        doc.autoTable({
            columns: columns,
            body: raw.Corpses,
            startY: 76,
            theme: 'striped',
            styles: { font: 'Roboto', fontSize: 9, cellPadding: 3, valign: 'middle' },
            headStyles: { fillColor: [15, 23, 42], textColor: [255, 255, 255], fontStyle: 'bold' },
            alternateRowStyles: { fillColor: [248, 250, 252] },
            columnStyles: {
                CaseId: { cellWidth: 30, fontStyle: 'bold' },
                Name: { cellWidth: 50 },
                Status: { cellWidth: 30 },
                Storage: { cellWidth: 35 },
                DateAdmitted: { cellWidth: 35 }
            },
            didDrawPage: function (data) {
                var pageCount = doc.internal.getNumberOfPages();
                doc.setFont('Roboto', 'normal');
                doc.setFontSize(8);
                doc.setTextColor(148, 163, 184);
                doc.text('Trang ' + data.pageNumber + ' / ' + pageCount, 175, 280);
                doc.text('Tài liệu bảo mật nội bộ — MorgueManager System', 14, 280);
            }
        });
        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        doc.save('Bao_cao_tong_hop_' + dateStr + '.pdf');
    },

    exportUsersToExcel: function (dataJson) {
        var rawData = JSON.parse(dataJson);
        var data = rawData.map(function (item) {
            return {
                'Mã': item.Id,
                'Họ Tên': item.Name,
                'Email': item.Email,
                'Số Điện Thoại': item.Phone,
                'Vai Trò': item.Role,
                'Trạng Thái': item.Status,
                'Ngày Tham Gia': item.JoinDate
            };
        });
        var ws = XLSX.utils.json_to_sheet(data);
        var wscols = [
            { wch: 8 }, { wch: 22 }, { wch: 25 }, { wch: 18 }, { wch: 12 }, { wch: 15 }, { wch: 15 }
        ];
        ws['!cols'] = wscols;
        var wb = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(wb, ws, 'Danh sách nhân viên');
        var dateStr = new Date().toISOString().slice(0, 10).replace(/-/g, '');
        XLSX.writeFile(wb, 'Danh_sach_nhan_vien_' + dateStr + '.xlsx');
    }
};
