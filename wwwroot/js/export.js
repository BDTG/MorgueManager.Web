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

        try {
            var fontUrl = 'https://cdn.jsdelivr.net/gh/google/fonts@main/ofl/roboto/static/Roboto-Regular.ttf';
            var fontRes = await fetch(fontUrl);
            if (!fontRes.ok) throw new Error('CDN font loading failed');
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

            var boldUrl = 'https://cdn.jsdelivr.net/gh/google/fonts@main/ofl/roboto/static/Roboto-Bold.ttf';
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
                Age: { cellWidth: 12, halign: 'center' },
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
    }
};
