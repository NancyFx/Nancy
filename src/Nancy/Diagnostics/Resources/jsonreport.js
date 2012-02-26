/**
* Created by demis.bellot@gmail.com
* Open Source under the New BSD Licence: https://github.com/AjaxStack/AjaxStack/blob/master/LICENSE
*/

//for non-modern browsers i.e: <=IE8
!window.JSON && document.write(unescape('%3Cscript src=""http://ajax.cdnjs.com/ajax/libs/json2/20110223/json2.js""%3E%3C/script%3E'));

if (!_) var _ = {};
_.modelreport = (function () {
    var root = this, doc = document,
				$ = function (id) { return doc.getElementById(id); },
				$$ = function (sel) { return doc.getElementsByTagName(sel); },
				$each = function (fn) { for (var i = 0, len = this.length; i < len; i++) fn(i, this[i], this); },
				isIE = /msie/i.test(navigator.userAgent) && !/opera/i.test(navigator.userAgent);

    $.each = function (arr, fn) { $each.call(arr, fn); };

    var splitCase = function (t) { return typeof t != 'string' ? t : t.replace(/([A-Z]|[0-9]+)/g, ' $1').replace(/_/g, ' '); },
				uniqueKeys = function (m) { var h = {}; for (var i = 0, len = m.length; i < len; i++) for (var k in m[i]) h[k] = k; return h; },
				keys = function (o) { var a = []; for (var k in o) a.push(k); return a; }
    var tbls = [];

    function val(m) {
        if (m == null) return '';
        if (typeof m == 'number') return num(m);
        if (typeof m == 'string') return str(m);
        if (typeof m == 'boolean') return m ? 'true' : 'false';
        return m.length ? arr(m) : obj(m);
    }
    function num(m) { return m; }
    function str(m) {
        return m.substr(0, 6) == '/Date(' ? dfmt(date(m)) : m;
    }
    function date(s) { return new Date(parseFloat(/Date\(([^)]+)\)/.exec(s)[1])); }
    function pad(d) { return d < 10 ? '0' + d : d; }
    function dfmt(d) { return d.getFullYear() + '/' + pad(d.getMonth() + 1) + '/' + pad(d.getDate()); }
    function obj(m) {
        var sb = '<dl>';
        for (var k in m) sb += '<dt class="ib">' + splitCase(k) + '</dt><dd>' + val(m[k]) + '</dd>';
        sb += '</dl>';
        return sb;
    }
    function arr(m) {
        if (typeof m[0] == 'string' || typeof m[0] == 'number') return m.join('<br/> ');
        var id = tbls.length, h = uniqueKeys(m);
        var sb = '<table id="tbl-' + id + '"><caption></caption><thead><tr>';
        tbls.push(m);
        var i = 0;
        for (var k in h) sb += '<th id="h-' + id + '-' + (i++) + '"><b></b>' + splitCase(k) + '</th>';
        sb += '</tr></thead><tbody>' + makeRows(h, m) + '</tbody></table>';
        return sb;
    }

    function makeRows(h, m) {
        var sb = '';
        for (var r = 0, len = m.length; r < len; r++) {
            sb += '<tr>';
            var row = m[r];
            for (var k in h) sb += '<td>' + val(row[k]) + '</td>';
            sb += '</tr>';
        }
        return sb;
    }

    function setTableBody(tbody, html) {
        if (!isIE) { tbody.innerHTML = html; return; }
        var temp = tbody.ownerDocument.createElement('div');
        temp.innerHTML = '<table>' + html + '</table>';
        tbody.parentNode.replaceChild(temp.firstChild.firstChild, tbody);
    }

    function clearSel() {
        if (doc.selection && doc.selection.empty) doc.selection.empty();
        else if (root.getSelection) {
            var sel = root.getSelection();
            if (sel && sel.removeAllRanges) sel.removeAllRanges();
        }
    }

    function cmp(v1, v2) {
        var f1, f2, f1 = parseFloat(v1), f2 = parseFloat(v2);
        if (!isNaN(f1) && !isNaN(f2)) v1 = f1, v2 = f2;
        if (typeof v1 == 'string' && v1.substr(0, 6) == '/Date(') v1 = date(v1), v2 = date(v2);
        if (v1 == v2) return 0;
        return v1 > v2 ? 1 : -1;
    }

    function enc(html) {
        if (typeof html != 'string') return html;
        return html.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
    }

    function addEvent(obj, type, fn) {
        if (obj.attachEvent) {
            obj['e' + type + fn] = fn;
            obj[type + fn] = function () { obj['e' + type + fn](root.event); }
            obj.attachEvent('on' + type, obj[type + fn]);
        } else
            obj.addEventListener(type, fn, false);
    }

    addEvent(doc, 'click', function (e) {
        var e = e || root.event, el = e.target || e.srcElement, cls = el.className;
        if (el.tagName == 'B') el = el.parentNode;
        if (el.tagName != 'TH') return;
        el.className = cls == 'asc' ? 'desc' : (cls == 'desc' ? null : 'asc');
        $.each($$('TH'), function (i, th) { if (th == el) return; th.className = null; });
        clearSel();
        var ids = el.id.split('-'), tId = ids[1], cId = ids[2];
        var tbl = tbls[tId].slice(0), h = uniqueKeys(tbl), col = keys(h)[cId], tbody = el.parentNode.parentNode.nextSibling;
        if (!el.className) { setTableBody(tbody, makeRows(h, tbls[tId])); return; }
        var d = el.className == 'asc' ? 1 : -1;
        tbl.sort(function (a, b) { return cmp(a[col], b[col]) * d; });
        setTableBody(tbody, makeRows(h, tbl));
    });

    return function (model) {
        return val(model);
    };
})();
