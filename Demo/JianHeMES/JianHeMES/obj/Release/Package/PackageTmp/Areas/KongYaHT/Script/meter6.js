var timeout = 0;
$(function set() {


    vb = parseFloat($("#f1").val());
    vc = parseFloat($("#f2").val());



    $("#container").highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor: null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor: null
        },
        title: {
            text: ''
        },
        pane: {
            startAngle: -120,
            endAngle: 120,
            background: [{
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, '#FFF'],
                        [1, '#333']
                    ]
                },
                borderWidth: 0,
                outerRadius: '109%'
            }, {
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, '#333'],
                        [1, '#FFF']
                    ]
                },
                borderWidth: 1,
                outerRadius: '107%'
            }, {
                // default background
            }, {
                backgroundColor: '#DDD',
                borderWidth: 0,
                outerRadius: '105%',
                innerRadius: '103%'
            }]
        },

        // the value axis
        yAxis: {
            min: 0,
            max: 45,
            minorTickInterval: 'auto',
            minorTickWidth: 1,
            minorTickLength: 5,
            minorTickPosition: 'inside',
            minorTickColor: '#666',
            tickPixelInterval: 30,
            tickWidth: 2,
            tickPosition: 'inside',
            tickLength: 10,
            tickColor: '#666',

            labels: {
                step: 2,
                rotation: 'auto',
                style: {
                    color: '#19a0f5',//颜色
                    fontSize: '8px'  //字体
                }
            },
            title: {
                text: '℃'
            },
            plotBands: [
                {
                    from: 0,
                    to: 16,
                    color: '#DF5353', // red

                }, {
                    from: 16,
                    to: 28,
                    color: '#55BF3B',// green

                }, {
                    from: 28,
                    to: 45,
                    color: '#DF5353', // red


                }]
        },
        series: [{
            name: '温度',
            data: [vb],
            tooltip: {
                valueSuffix: ' ℃'
            },
            dataLabels: {
                /* enabled: true, */
                color: '#19a0f5',
                style: {
                    fontSize: '8px'  //字体
                }
            }
        }]
    },
   function (chart) {
       if (!chart.renderer.forExport) {
           setInterval(function () {
               var point = chart.series[0].points[0],
               newVal;
               newVal = point.y + vb;
               if (newVal < 0 || newVal > 45) {
                   newVal = point.y - vb;
               }
               point.update(newVal);
           },60000);
       }
   });


    $('#container2').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor: null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor: null
        },
        title: {
            text: ''
        },
        pane: {
            startAngle: -120,
            endAngle: 120,
            background: [{
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, '#FFF'],
                        [1, '#333']
                    ]
                },
                borderWidth: 0,
                outerRadius: '109%'
            }, {
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, '#333'],
                        [1, '#FFF']
                    ]
                },
                borderWidth: 1,
                outerRadius: '107%'
            }, {
                // default background
            }, {
                backgroundColor: '#DDD',
                borderWidth: 0,
                outerRadius: '105%',
                innerRadius: '103%'
            }]
        },
        // the value axis
        yAxis: {
            min: 0,
            max: 100,
            minorTickInterval: 'auto',
            minorTickWidth: 1,
            minorTickLength: 5,
            minorTickPosition: 'inside',
            minorTickColor: '#666',
            tickPixelInterval: 30,
            tickWidth: 2,
            tickPosition: 'inside',
            tickLength: 10,
            tickColor: '#666',

            labels: {
                step: 1,
                rotation: 'auto',
                style: {
                    color: '#19a0f5',//颜色
                    fontSize: '8px'  //字体
                }
            },
            title: {
                text: '%RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 45,
                    color: '#DF5353', // red

                }, {
                    from: 45,
                    to: 70,
                    color: '#55BF3B',// green

                }, {
                    from: 70,
                    to: 100,
                    color: '#DF5353', // red


                }]
        },
        series: [{
            name: '湿度',
            data: [vc],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
                color: '#19a0f5',
                style: {
                    fontSize: '8px'  //字体
                }

            }

        }]

    },
    function (chart) {
        if (!chart.renderer.forExport) {
            setInterval(function () {
                var point = chart.series[0].points[0],
                newVal;
                newVal = point.y + vc;
                if (newVal < 0 || newVal > 100) {
                    newVal = point.y - vc;
                }
                point.update(newVal);
            }, 60000);
        }
    });



    $("#hider").highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor: null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor: null
        },
        title: {
            text: ''
        },
        pane: {
            startAngle: -120,
            endAngle: 120,
            background: [{
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, '#FFF'],
                        [1, '#333']
                    ]
                },
                borderWidth: 0,
                outerRadius: '109%'
            }, {
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, '#333'],
                        [1, '#FFF']
                    ]
                },
                borderWidth: 1,
                outerRadius: '107%'
            }, {
                // default background
            }, {
                backgroundColor: '#DDD',
                borderWidth: 0,
                outerRadius: '105%',
                innerRadius: '103%'
            }]
        },

        yAxis: {
            min: 0,
            max: 45,
            minorTickInterval: 'auto',
            minorTickWidth: 1,
            minorTickLength: 5,
            minorTickPosition: 'inside',
            minorTickColor: '#666',
            tickPixelInterval: 30,
            tickWidth: 2,
            tickPosition: 'inside',
            tickLength: 10,
            tickColor: '#666',

            labels: {
                step: 2,
                rotation: 'auto',
                style: {
                    color: '#19a0f5',//颜色
                    fontSize: '8px'  //字体
                }
            },
            title: {
                text: '℃'
            },
            plotBands: [
                {
                    from: 0,
                    to: 16,
                    color: '#DF5353', // red

                }, {
                    from: 16,
                    to: 28,
                    color: '#55BF3B',// green

                }, {
                    from: 28,
                    to: 45,
                    color: '#DF5353', // red


                }]
        },
        series: [{
            name: '温度',
            data: [vb],
            tooltip: {
                valueSuffix: ' ℃'
            },
            dataLabels: {
                /* enabled: true, */
                color: '#19a0f5',
                style: {
                    fontSize: '11px'  //字体
                }
            }

        }]

    },
    function (chart) {
        if (!chart.renderer.forExport) {
            setInterval(function () {
                var point = chart.series[0].points[0],
                newVal;
                newVal = point.y + vb;
                if (newVal < 0 || newVal > 45) {
                    newVal = point.y - vb;
                }
                point.update(newVal);
            }, 60000);
        }
    });

    $("#hider").hide();
    $("#container").click(function () {
        $("#container").hide("1000");
        $("#container2").hide("1000");
        $("#hider").show("slow");
    });
    $("#hider").click(function () {
        $("#container").show("1000");
        $("#container2").show("1000");

        $("#hider").hide("slow");
    });

    $('#hider2').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor: null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor: null
        },
        title: {
            text: ''
        },
        pane: {
            startAngle: -120,
            endAngle: 120,
            background: [{
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, '#FFF'],
                        [1, '#333']
                    ]
                },
                borderWidth: 0,
                outerRadius: '109%'
            }, {
                backgroundColor: {
                    linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                    stops: [
                        [0, '#333'],
                        [1, '#FFF']
                    ]
                },
                borderWidth: 1,
                outerRadius: '107%'
            }, {
                // default background
            }, {
                backgroundColor: '#DDD',
                borderWidth: 0,
                outerRadius: '105%',
                innerRadius: '103%'
            }]
        },
        // the value axis
        yAxis: {
            min: 0,
            max: 100,
            minorTickInterval: 'auto',
            minorTickWidth: 1,
            minorTickLength: 5,
            minorTickPosition: 'inside',
            minorTickColor: '#666',
            tickPixelInterval: 30,
            tickWidth: 2,
            tickPosition: 'inside',
            tickLength: 10,
            tickColor: '#666',

            labels: {
                step: 1,
                rotation: 'auto',
                style: {
                    color: '#19a0f5',//颜色
                    fontSize: '8px'  //字体
                }
            },
            title: {
                text: '%RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 45,
                    color: '#DF5353', // red

                }, {
                    from: 45,
                    to: 70,
                    color: '#55BF3B',// green

                }, {
                    from: 70,
                    to: 100,
                    color: '#DF5353', // red


                }]
        },
        series: [{
            name: '湿度',
            data: [vc],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
                /* enabled: true, */
                color: '#19a0f5',
                style: {
                    fontSize: '11px'  //字体
                }
            }

        }]

    },
    function (chart) {
        if (!chart.renderer.forExport) {
            setInterval(function () {
                var point = chart.series[0].points[0],
                newVal;
                newVal = point.y + vc;
                if (newVal < 0 || newVal > 100) {
                    newVal = point.y - vc;
                }
                point.update(newVal);
            }, 60000);
        }
    });

    $("#hider2").hide();
    $("#container2").click(function () {
        $("#container").hide("1000");
        $("#container2").hide("1000");
        $("#hider2").show("slow");
    });
    $("#hider2").click(function () {
        $("#container").show("1000");
        $("#container2").show("1000");

        $("#hider2").hide("slow");
    });

    if (timeout == 0) {
        setTimeout(function () {
            set();
        }, 2000);
        timeout = 1;
    }
    //setInterval(function () {
    //    set();
    //}, 120000);


});


