﻿
var my_gantt = function () {
    gantt.config.date_format = "%Y-%m-%d %H:%i:%s";
    gantt.config.task_height = 16;
    gantt.config.row_height = 40;
    gantt.config.scale_height = 90;
    gantt.locale.labels.baseline_enable_button = '设置';
    gantt.locale.labels.baseline_disable_button = '移除';
    gantt.config.static_background = true;

    //配置左侧栏
    gantt.config.columns = [
        { name: "text", label: "工段名", tree: true, width: 120, resize: true },
        {
            name: "start_date",
            label: "计划开始时间",
            align: "center",
            width: 120,
            resize: true
        },
        {
            name: "end_date",
            label: "计划结束时间",
            align: "center",
            width: 120,
            resize: true
        },
        { name: "add", label: "", width: 44 }
    ];
    //配置弹出框计划时间
    gantt.config.lightbox.sections = [{
        name: "description",
        height: 25,
        map_to: "text",
        type: "textarea",
        focus: true
    },
    {
        name: "time",
        map_to: "auto",
        type: "time"
    },
    {
        name: "baseline",
        map_to: {
            start_date: "actual_start",
            end_date: "actual_end"
        },
        button: true,
        type: "time_optional"
    }
    ];
    gantt.locale.labels.section_baseline = "实际时间";

    //时间轴设置
    gantt.config.min_column_width = 20;
    gantt.config.scale_height = 20 * 3;
    var daysStyle = function (date) {
        if (date.getDay() === 0 || date.getDay() === 6) {
            return "weekend";
        }
        return "";
    };

    gantt.config.scales = [
        { unit: "month", step: 1, format: "%Y, %F" },
        { unit: "day", step: 1, format: "%d,%l", css: daysStyle },
        { unit: "hour", step: 4, format: "%G", css: daysStyle }
    ];
    gantt.attachEvent("onBeforeGanttRender", function () {
        var range = gantt.getSubtaskDates();
        var range_data = gantt.$data.tasksStore.pull;
        //console.log(range_data);
        var actual_data = [];
        for (let key in range_data) {
            actual_data.push(range_data[key].actual_start);
            actual_data.push(range_data[key].actual_end);
        }
        //console.log(actual_data);
        var scaleUnit = gantt.getState().scale_unit;
        if (actual_data && range) {
            var actual_max = new Date(Math.max(...actual_data));
            var actual_min = new Date(Math.min(...actual_data));
            //console.log('actual_max', actual_max);
            //console.log('actual_min', actual_min);
            if (range.start_date && range.end_date) {
                gantt.config.start_date = gantt.calculateEndDate(new Date(Math.min(range.start_date, actual_min)), -24, scaleUnit);
                gantt.config.end_date = gantt.calculateEndDate(new Date(Math.max(range.end_date, actual_max)), 24, scaleUnit);
            }
        } else {
            if (range.start_date && range.end_date) {
                gantt.config.start_date = gantt.calculateEndDate(range.start_date, -24, scaleUnit);
                gantt.config.end_date = gantt.calculateEndDate(range.end_date, 24, scaleUnit);
            }
        }
    });
    //显示进度
    gantt.templates.progress_text = function (start, end, task) {
        return "<span style='text-align:left;'>" + Math.round(task.progress * 100) + "% </span>";
    };

    //设置周末颜色
    gantt.templates.scale_cell_class = function (date) {
        if (date.getDay() == 0 || date.getDay() == 6) {
            return "weekend";
        }
    };
    gantt.templates.timeline_cell_class = function (item, date) {
        if (date.getDay() == 0 || date.getDay() == 6) {
            return "weekend"
        }
    };
    //提示工具
    function linkTypeToString(linkType) {
        switch (linkType) {
            case gantt.config.links.start_to_start:
                return "开始-开始";
            case gantt.config.links.start_to_finish:
                return "开始-结束";
            case gantt.config.links.finish_to_start:
                return "结束-开始";
            case gantt.config.links.finish_to_finish:
                return "结束-结束";
            default:
                return ""
        }
    }
    gantt.plugins({
        tooltip: true
    });
    gantt.templates.tooltip_date_format = gantt.date.date_to_str("%m-%d  %H:%i");
    gantt.attachEvent("onGanttReady", function () {
        var tooltips = gantt.ext.tooltips;

        gantt.templates.tooltip_text = function (start, end, task) {
            if (task.actual_start == undefined || task.actual_end == null) {
                return "<b>工段名:</b> " + task.text + "<br/>"
                    + "<b>计划时间:</b> " + gantt.templates.tooltip_date_format(start) + ' ~ ' + gantt.templates.tooltip_date_format(end)
                    + "<br/><b>计划天数:</b> " + task.duration;
            } else {
                return "<b>工段名:</b> " + task.text + "<br/>" +
                    "<b>计划时间:</b> " + gantt.templates.tooltip_date_format(start) + ' ~ ' + gantt.templates.tooltip_date_format(end)
                    + "<br/><b>计划天数:</b> " + task.duration
                    + "<br/><b>实际时间:</b> " + gantt.templates.tooltip_date_format(task.actual_start) + ' ~ ' + gantt.templates.tooltip_date_format(task.actual_end);
            }
        };
        tooltips.tooltipFor({
            selector: ".gantt_task_link",
            html: function (event, node) {

                var linkId = node.getAttribute(gantt.config.link_attribute);
                if (linkId) {
                    var link = gantt.getLink(linkId);
                    var from = gantt.getTask(link.source);
                    var to = gantt.getTask(link.target);

                    return [
                        "<b>链接类型:</b> " + linkTypeToString(link.type),
                        "<b>链接内容:</b> " + from.text + '~' + to.text
                    ].join("<br>");
                }
            }
        });
    });
    // 添加实际日期层
    gantt.addTaskLayer({
        renderer: {
            render: function draw_planned(task) {
                if (task.actual_start && task.actual_end) {
                    var sizes = gantt.getTaskPosition(task, task.actual_start, task.actual_end);
                    var el = document.createElement('div');
                    el.className = 'baseline';
                    el.style.left = sizes.left + 'px';
                    el.style.width = sizes.width + 'px';
                    el.style.top = sizes.top + gantt.config.task_height + 13 + 'px';
                    return el;
                }
                return false;
            },
            // define getRectangle in order to hook layer with the smart rendering
            getRectangle: function (task, view) {
                if (task.actual_start && task.actual_end) {
                    return gantt.getTaskPosition(task, task.actual_start, task.actual_end);
                }
                return null;
            }
        }
    });

    //逾期日期计算
    gantt.templates.task_class = function (start, end, task) {
        if (task.actual_end) {
            var classes = ['has-baseline'];
            if (task.actual_end.getTime() > end.getTime()) {
                classes.push('overdue');
            }
            return classes.join(' ');
        }
    };

    gantt.templates.rightside_text = function (start, end, task) {
        if (task.actual_end) {
            if (task.actual_end.getTime() > end.getTime()) {
                var overdue = Math.ceil(Math.abs((task.actual_end.getTime() - end.getTime()) / (24 * 60 * 60 *
                    1000)));
                var text = "<b>逾期: " + overdue + " 天</b>";
                return text;
            }
        }
    };


    gantt.attachEvent("onTaskLoading", function (task) {
        task.actual_start = gantt.date.parseDate(task.actual_start, "xml_date");
        task.actual_end = gantt.date.parseDate(task.actual_end, "xml_date");
        return true;
    });

}