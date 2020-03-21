/*
 * defining enum class
 */
function Enum() {
    this._enums = [];
    this._lookups = {};
}

Enum.prototype.getEnums = function () {
    return _enums;
}

Enum.prototype.forEach = function (callback) {
    var length = this._enums.length;
    for (var i = 0; i < length; ++i) {
        callback(this._enums[i]);
    }
}

Enum.prototype.addEnum = function (e) {
    this._enums.push(e);
}

Enum.prototype.getByName = function (name) {
    return this[name];
}

Enum.prototype.getByValue = function (field, value) {
    //var lookup = this._lookups[field];
    //if (lookup) {
    //    return lookup[value];
    //} else {
    //    this._lookups[field] = (lookup = {});
    var k = this._enums.length - 1;
    for (; k >= 0; --k) {
        var m = this._enums[k];
        var j = m[field];
        //lookup[j] = m;
        if (j == value) {
            return m;
        }
    }
    //}
    return null;
}

function defineEnum(definition) {
    var k;
    var e = new Enum();
    for (k in definition) {
        var j = definition[k];
        e[k] = j;
        e.addEnum(j)
    }
    return e;
}

google.charts.load('current', { 'packages': ['corechart', 'bar'] });

window.summaryObj = new Object();
window.displayedId = null;
//window.STATUS = null;
//Object.defineProperties(window.STATUS, 
//    {
//        Done: { value: -1, writable: false },
//        Pass: { value: 0, writable: false },
//        Warning: { value: 1, writable: false },
//        Fail: { value: 2, writable: false },
//        Fatal: { value: 3, writable: false },
//        FatalWithEx: { value: 4, writable: false },
//        names: { value: ['Done', 'Pass', 'Warning', 'Fail', 'Fatal', 'FatalWithEx'], writable: false }
//    }
//);
window.STATUS = defineEnum({
    Done: { value: -1, name: "Done", description: "Done" },
    Pass: { value: 0, name: "Pass", description: "Pass" },
    Warning: { value: 1, name: "Warning", description: "Warning" },
    Fail: { value: 2, name: "Fail", description: "Fail" },
    Error: { value: 3, name: "Error", description: "Failure in application" },
    Fatal: { value: 4, name: "Fatal", description: "Failure in automation" }
});

function triggerEvent(elem, event) {
    var clickEvent = new Event(event); // Create the event.
    elem.dispatchEvent(clickEvent);    // Dispatch the event.
}

window.onload = function (sender) {
    window.summaryObj = JSON.parse($('input[name="testRunSummaryObj"]').first().val());
    //window.summaryObj = JSON.parse(document.getElementsByName('testRunSummaryObj')[0].value);

    // document.querySelectorAll('body>nav>div.treeview>label>span[for]');
    //var navItems = $('body>nav>div.treeview label>span[for]'); 
    //navItems.on("click", function () {
    //    timer = setTimeout(function () {
    //        if (!prevent) {
    //            onTestLinkClick(this);
    //        }
    //        prevent = false;
    //    }, delay);
    //})
    //.on("dblclick", function () {
    //    clearTimeout(timer);
    //    prevent = true;
    //    onTestLinkDblClick(this);
    //});
    //    navItems.forEach(function (elem) {
    //        elem.on("click", function () {
    //            timer = setTimeout(function () {
    //                if (!prevent) {
    //                    onTestLinkClick(elem);
    //                }
    //                prevent = false;
    //            }, delay);
    //        })
    //        .on("dblclick", function () {
    //            clearTimeout(timer);
    //            prevent = true;
    //            onTestLinkDblClick(elem);
    //        });
    //    });

    //var navRoot = $('body>nav>div.treeview>label>span[for="/report"]').first();
    //navRoot.click();
    //triggerEvent(navRoot, 'click');
    //onTestLinkClick(navRoot);
    //initializeTestRunSummary();



    //drawCharts();
}
var DELAY = 500,
    clicks = 0,
    timer = null;
$(window).ready(function () {
    $('body>nav>div.treeview *[for]').on("click", null, this, function (e) {
        clicks++;  //count clicks
        timer = setTimeout(function () {
            if (clicks === 1) {
                //alert('Single Click'); //perform single-click action
                onTestLinkClick(e.currentTarget);
            } else if (clicks === 2) {
                //alert('Double Click'); //perform single-click action
                onTestLinkDblClick(e.currentTarget);
            } else if (clicks >= 3) {
                //alert('Triple Click'); //perform Triple-click action
            }
            clearTimeout(timer);
            clicks = 0;  //after action performed, reset counter
        }, DELAY);
    }).on("dblclick", function (e) {
        e.preventDefault();  //cancel system double-click event
    });

    $('body>nav>div.treeview>label>span[for="report"]').first().click();

    //drawCharts();
});

function initializeTestRunSummary() {
    document.getElementById('summaryTitleKey').innerText = "Test Run Name: ";
    document.getElementById('summaryTitleVal').innerText = window.summaryObj.name;

    var date = new Date(0);
    date.setMilliseconds(window.summaryObj.startTime);
    document.getElementById('testRunStartTimeVal').innerText = date.toString();
    date = new Date(0);
    date.setMilliseconds(window.summaryObj.endTime);
    document.getElementById('testRunEndTimeVal').innerText = date.toString();
    document.getElementById('testRunDurationVal').innerText = calcDuration(window.summaryObj.startTime, window.summaryObj.endTime);
    document.getElementById('testRunNumofTestsVal').innerText = window.summaryObj.numofChilds;
    document.getElementById('testRunNumofPassVal').innerText = window.summaryObj.numofPasses;
    document.getElementById('testRunNumofWarningVal').innerText = window.summaryObj.numofWarnings;
    document.getElementById('testRunNumofFailVal').innerText = window.summaryObj.numofFails + window.summaryObj.numofFatals;
}

function onTestLinkClick(sender) {
    var selectedItem;

    //var bIsFromTableCell = false;

    var id = sender.getAttribute('for');

    //var attrib = sender.getAttribute('class');
    //if (attrib != null)
    //{
    //    selectedItem = document.querySelector('body>nav>div.treeview #selectedItem');
    //    selectedItem.tagName
    //}

    if (id == 'report') {
        window.summaryObj = JSON.parse($("input:hidden[name='testRunSummaryObj']").val());

        selectedItem = $("body>nav>div.treeview #selectedItem");
        if (selectedItem.length > 0)
            selectedItem.removeAttr('id');

        selectedItem = $("body>nav>div.treeview span[for='" + id + "']");
        if (selectedItem.length > 0)
            selectedItem.prop('id', "selectedItem");
        else {
            selectedItem = $("body>nav>div.treeview a[for='" + id + "']");
            if (selectedItem.length > 0)
                selectedItem.prop('id', "selectedItem");
        }

        initializeTestRunSummary();

        $('#testSummary').first().prop('style').display = 'none';
        $('#testRunSummary').first().prop('style').display = '';
    }
    else {
        selectedItem = $("body>nav>div.treeview #selectedItem");

        if (selectedItem.attr("for") == id)
            return;

        window.summaryObj = JSON.parse($("input:hidden[name='testSummaryObj_" + id + "']").val());

        if (selectedItem.length > 0) {
            if (sender.className == 'rTableCell') {
                selectedItem = selectedItem.parent().siblings("input:checkbox");
                if (selectedItem.length > 0)
                    selectedItem.first().prop('checked', true);
            }

            $("body>nav>div.treeview #selectedItem").removeAttr('id');
        }

        selectedItem = $("li[id='" + id + "']>label>span");
        if (selectedItem.length > 0)
            selectedItem.prop('id', 'selectedItem');
        else {
            selectedItem = $("li[id='" + id + "']>a");
            if (selectedItem.length > 0)
                selectedItem.prop('id', 'selectedItem');
        }

        initializeTestSummary();

        $('#testRunSummary').first().prop('style').display = 'none';
        $('#testSummary').first().prop('style').display = '';
    }

    //    if (window.displayedId != null) {
    //        $('#divTablePlaceholder').empty();
    //    }

    $('#divTablePlaceholder').empty();
    $('#divTablePlaceholder').prepend($('div#' + id + '_Table').html());
    //    document.getElementById('divTablePlaceholder').innerHTML = $('div#' + id + '_Table').html();
    window.displayedId = id;

    drawCharts();
}

function onTestLinkDblClick(sender) {
    var expandElement = $(sender).parent().siblings("input:checkbox").first();

    if (expandElement.prop("checked"))
        expandElement.prop('checked', false);
    else
        expandElement.prop('checked', true);
}

function initializeTestSummary() {
    document.getElementById('summaryTitleKey').innerText = "Test Name: ";
    document.getElementById('summaryTitleVal').innerText = window.summaryObj.name;

    var date = new Date(0);
    date.setMilliseconds(window.summaryObj.startTime);
    document.getElementById('testStartTimeVal').innerText = date.toString();
    date = new Date(0);
    date.setMilliseconds(window.summaryObj.endTime);
    document.getElementById('testEndTimeVal').innerText = date.toString();
    document.getElementById('testDurationVal').innerText = calcDuration(window.summaryObj.startTime, window.summaryObj.endTime);
    document.getElementById('testNumofStepsVal').innerText = window.summaryObj.numofChilds;
    document.getElementById('testStatus').className = 'testStatus';
    var status = getStatus(parseInt(window.summaryObj.status));
    document.getElementById('testStatus').classList.add('step' + status.name);
    document.getElementById('testStatusVal').innerText = status.description;
    document.getElementById('testDescriptionVal').innerText = window.summaryObj.description;
    document.getElementById('testCategoriesVal').innerText = window.summaryObj.categories;
    document.getElementById('testClassVal').innerText = window.summaryObj.className + ' (Assembly: ' + window.summaryObj.assembly + ')';
    document.getElementById('testClassDescriptionVal').innerText = window.summaryObj.classDescription;
}

function drawCharts() {
    var data = google.visualization.arrayToDataTable([
        ['Status', 'Count'],
        ['Pass', window.summaryObj.numofPasses],
        ['Warning', window.summaryObj.numofWarnings],
        ['Fail', window.summaryObj.numofFails],
        ['Fatal', window.summaryObj.numofFatals],
    ]);

    var options = {
        title: 'Tests Overview',
        chartArea: { left: 50, top: 0, width: '150%', height: '175%' },
        slices: {
            0: { color: 'green' },
            1: { color: 'orange' },
            2: { color: 'red' },
            3: { color: '#990000' },

        }
    };

    var chart = new google.visualization.PieChart(document.getElementById('graph'));

    chart.draw(data, options);
}

function getColorByStatus(statusName) {
    var color;

    switch (statusName) {
        case 'Done':
            color = 'black';
            break;
        case 'Pass':
            color = 'green';
            break;
        case 'Warning':
            color = 'orange';
            break;
        case 'Fail':
            color = 'red';
            break;
        case 'Error':
            color = 'red';
            break;
        case 'Fatal':
            color = 'red';
            break;
        default:
            color = 'black';
    }

    return color;
}

function getSignByStatus(statusName) {
    var sign;

    switch (statusName) {
        case 'Done':
            sign = '&#128077; ';
            break;
        case 'Pass':
            sign = '&#10004; ';
            break;
        case 'Warning':
            sign = '&#9888; ';
            break;
        case 'Fail':
            sign = '&#10006; ';
            break;
        case 'Error':
            sign = '&#10006; ';
            break;
        case 'Fatal':
            sign = '&#10006; ';
            break;
        default:
            sign = '';
    }

    return sign;
}

function showModal(sender) {
    var modal = document.getElementById('modal');
    var modalImg = document.getElementById('modalImg');
    var modalCaption = document.getElementById('caption');

    modalImg.src = sender.src;
    modalCaption.innerText = sender.title;
    modal.style.display = 'block';
}

function closeModal() {
    var modal = document.getElementById('modal');
    var modalImg = document.getElementById('modalImg');
    var modalCaption = document.getElementById('caption');

    modal.style.display = 'none';
    modalImg.attributes.removeNamedItem('src');
    modalCaption.innerText = '';
}

function calcDuration(startTimeDate, endTimeDate) {
    var duration = endTimeDate - startTimeDate;

    var milliseconds = parseInt(duration % 1000)
        , seconds = parseInt((duration / 1000) % 60)
        , minutes = parseInt((duration / (1000 * 60)) % 60)
        , hours = parseInt((duration / (1000 * 60 * 60)) % 24);

    hours = (hours < 10) ? "0" + hours : hours;
    minutes = (minutes < 10) ? "0" + minutes : minutes;
    seconds = (seconds < 10) ? "0" + seconds : seconds;

    return hours + ":" + minutes + ":" + seconds + "." + milliseconds;
    //var startHour = startTimeDate.getHours();
    //var endHour = endTimeDate.getHours();
    //var startMins = startTimeDate.getMinutes();
    //var endMins = endTimeDate.getMinutes();
    //var startSecs = startTimeDate.getSeconds();
    //var endSecs = endTimeDate.getSeconds();
    ////var startMillisecs = startTimeDate.getMilliseconds();
    ////var endMillisecs = endTimeDate.getMilliseconds();

    ////var msDiff = endMillisecs - startMillisecs;
    //var secDiff = endSecs - startSecs;
    //var minDiff = endMins - startMins;
    //var hrDiff = endHour - startHour;
    //if (hrDiff < 10) { hrDiff = '0' + hrDiff; }
    //if (minDiff < 10) { minDiff = '0' + minDiff; }
    //if (secDiff < 10) { secDiff = '0' + secDiff; }

    //return (hrDiff + ':' + minDiff + ':' + secDiff);
}

function getStatusName(value) {
    switch (value) {
        case -1: return 'Done';
        case 0: return 'Pass';
        case 1: return 'Warning';
        case 2: return 'Fail';
        case 3: return 'Error';
        case 4: return 'Fatal';
        default: return null;
    }
}

function getStatusValue(name) {
    switch (name) {
        case 'Done': return -1;
        case 'Pass': return 0;
        case 'Warning': return 1;
        case 'Fail': return 2;
        case 'Error': return 3;
        case 'Fatal': return 4;
        default: return null;
    }
}

function getStatus(val) {
    try {
        if (isNaN(val))
            return window.STATUS.getByName(val); // getStatusValue(val);
        else
            return window.STATUS.getByValue('value', val);// getStatusName(val);
    }
    catch (err) { return null; }
}

/*
function getStatusClass(status) {
    return 'step' + status;
}
*/
/*
function updateTestLinksColor() {
    var links = document.getElementsByTagName('a');

    for (var i = 0, link, testElementStatus; i < links.length; i++) {
        link = links[i];
        if (link.nextElementSibling && link.nextElementSibling.tagName == 'TEST') {
            testElementStatus = link.nextElementSibling.getAttribute('status');
            link.style.color = getColorByStatus(testElementStatus);
            link.innerHTML = getSignByStatus(testElementStatus) + ' ' + link.innerHTML;

            // Update test parent:
            var labelElement = link.parentElement.parentElement.previousElementSibling;
            if (labelElement != null && labelElement.tagName == 'LABEL' && labelElement.innerText != 'Tests') {
                if (labelElement.style.color != 'red' && labelElement.style.color != 'orange' && labelElement.style.color != getColorByStatus(testElementStatus)) {
                    // 1 - Change text color:
                    labelElement.style.color = getColorByStatus(testElementStatus);

                    // 2 - Chanage sign:
                    var newSpan = document.createElement('span');
                    newSpan.innerHTML = getSignByStatus(testElementStatus) + '  ';
                    var labelFirstChild = labelElement.firstChild;

                    if (labelFirstChild != null && labelFirstChild.nodeName == 'SPAN') // replace span element
                        labelElement.replaceChild(newSpan, labelElement.firstChild);
                    else
                        labelElement.insertBefore(newSpan, labelElement.firstChild);   // add span element
                }
            }
        }
    }
}
*/