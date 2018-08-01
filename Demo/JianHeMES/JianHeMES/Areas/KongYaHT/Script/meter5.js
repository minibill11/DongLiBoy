var timeout = 0;
$(function set() {

    vb = parseFloat($("#f1").val());
    vc = parseFloat($("#f2").val());
    vb2 = parseFloat($("#f3").val());
    vc2 = parseFloat($("#f4").val());
    ma = parseFloat($("#f5").val());
    mb = parseFloat($("#f6").val());
    ma2 = parseFloat($("#f7").val());
    mb2 = parseFloat($("#f8").val());
    ma3 = parseFloat($("#f9").val());
    mb3 = parseFloat($("#f10").val());
    ma4 = parseFloat($("#f11").val());
    mb4 = parseFloat($("#f12").val());
    ma5 = parseFloat($("#f13").val());
    mb5 = parseFloat($("#f14").val());
    ma6 = parseFloat($("#f15").val());
    mb6 = parseFloat($("#f16").val());
    ma7 = parseFloat($("#f17").val());
    mb7 = parseFloat($("#f18").val());
    ma8 = parseFloat($("#f19").val());
    mb8 = parseFloat($("#f20").val());
    ma9 = parseFloat($("#f21").val());
    mb9 = parseFloat($("#f22").val());
    ma10 = parseFloat($("#f23").val());
    mb10 = parseFloat($("#f24").val());


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
           }, 60000);
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

    $('#container3').highcharts({
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
            data: [vb2],
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
                newVal = point.y + vb2;
                if (newVal < 0 || newVal > 45) {
                    newVal = point.y - vb2;
                }
                point.update(newVal);
            }, 60000);
        }
    });


    $('#container4').highcharts({
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
            data: [vc2],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVal = point.y + vc2;
                if (newVal < 0 || newVal > 100) {
                    newVal = point.y - vc2;
                }
                point.update(newVal);
            }, 60000);
        }
    });

    $('#container5').highcharts({
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
            data: [ma],
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
                newVab,
                newVab = ma;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container6').highcharts({
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
            data: [mb],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container7').highcharts({
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
            data: [ma2],
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
                newVab,
                newVab = ma2;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container8').highcharts({
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
            data: [mb2],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb2;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container9').highcharts({
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
            data: [ma3],
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
                newVab,
                newVab = ma3;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container10').highcharts({
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
            data: [mb3],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb3;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container11').highcharts({
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
            data: [ma4],
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
                newVab,
                newVab = ma4;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container12').highcharts({
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
            data: [mb4],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb4;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container13').highcharts({
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
            data: [ma5],
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
                newVab,
                newVab = ma5;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container14').highcharts({
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
            data: [mb5],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb5;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container15').highcharts({
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
            data: [ma6],
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
                newVab,
                newVab = ma6;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container16').highcharts({
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
            data: [mb6],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb6;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container17').highcharts({
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
            data: [ma7],
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
                newVab,
                newVab = ma7;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container18').highcharts({
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
            data: [mb7],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb7;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container19').highcharts({
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
            data: [ma8],
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
                newVab,
                newVab = ma8;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container20').highcharts({
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
            data: [mb8],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb8;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container21').highcharts({
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
            data: [ma9],
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
                newVab,
                newVab = ma9;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container22').highcharts({
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
            data: [mb9],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb9;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container23').highcharts({
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
            data: [ma10],
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
                newVab,
                newVab = ma10;
                point.update(newVab);
            }, 60000);
        }
    });

    $('#container24').highcharts({
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
            data: [mb10],
            tooltip: {
                valueSuffix: ' %RH'
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
                newVab,
                newVab = mb10;
                point.update(newVab);
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


    $("#hider3").highcharts({
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
            data: [vb2],
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
                newVzc,
                newVzc = vb2;
                point.update(newVzc);
            }, 60000);
        }
    });

    $("#hider3").hide();
    $("#container3").click(function () {
        $("#container3").hide("1000");
        $("#container4").hide("1000");
        $("#hider3").show("slow");
    });
    $("#hider3").click(function () {
        $("#container3").show("1000");
        $("#container4").show("1000");
        $("#hider3").hide("slow");
    });

    $('#hider4').highcharts({
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
            data: [vc2],
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
                newVxc,
                newVxc = vc2;
                point.update(newVxc);
            }, 60000);
        }
    });


    $("#hider4").hide();
    $("#container4").click(function () {
        $("#container3").hide("1000");
        $("#container4").hide("1000");
        $("#hider4").show("slow");
    });
    $("#hider4").click(function () {
        $("#container3").show("1000");
        $("#container4").show("1000");

        $("#hider4").hide("slow");
    });

    $("#hider5").highcharts({
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
            data: [ma],
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
                newVzc,
                newVzc = ma;
                point.update(newVzc);
            }, 60000);
        }
    });

    $("#hider5").hide();
    $("#container5").click(function () {
        $("#container5").hide("1000");
        $("#container6").hide("1000");
        $("#hider5").show("slow");
    });
    $("#hider5").click(function () {
        $("#container5").show("1000");
        $("#container6").show("1000");
        $("#hider5").hide("slow");
    });

    $('#hider6').highcharts({
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
            data: [mb],
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
                newVxc,
                newVxc = mb;
                point.update(newVxc);
            }, 60000);
        }
    });


    $("#hider6").hide();
    $("#container6").click(function () {
        $("#container5").hide("1000");
        $("#container6").hide("1000");
        $("#hider6").show("slow");
    });
    $("#hider6").click(function () {
        $("#container5").show("1000");
        $("#container6").show("1000");

        $("#hider6").hide("slow");
    });

    $('#hider7').highcharts({
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
            data: [ma2],
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
                newVxc,
                newVxc = ma2;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider7").hide();
    $("#container7").click(function () {
        $("#container7").hide("1000");
        $("#container8").hide("1000");
        $("#hider7").show("slow");
    });
    $("#hider7").click(function () {
        $("#container7").show("1000");
        $("#container8").show("1000");

        $("#hider7").hide("slow");
    });

    $('#hider8').highcharts({
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
            data: [mb2],
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
                newVxc,
                newVxc = mb2;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider8").hide();
    $("#container8").click(function () {
        $("#container7").hide("1000");
        $("#container8").hide("1000");
        $("#hider8").show("slow");
    });
    $("#hider8").click(function () {
        $("#container7").show("1000");
        $("#container8").show("1000");

        $("#hider8").hide("slow");
    });

    $('#hider9').highcharts({
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
            data: [ma3],
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
                newVxc,
                newVxc = ma3;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider9").hide();
    $("#container9").click(function () {
        $("#container9").hide("1000");
        $("#container10").hide("1000");
        $("#hider9").show("slow");
    });
    $("#hider9").click(function () {
        $("#container9").show("1000");
        $("#container10").show("1000");
        $("#hider9").hide("slow");
    });

    $('#hider10').highcharts({
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
            data: [mb3],
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
                newVxc,
                newVxc = mb3;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider10").hide();
    $("#container10").click(function () {
        $("#container9").hide("1000");
        $("#container10").hide("1000");
        $("#hider10").show("slow");
    });
    $("#hider10").click(function () {
        $("#container9").show("1000");
        $("#container10").show("1000");
        $("#hider10").hide("slow");
    });

    $('#hider11').highcharts({
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
            data: [ma4],
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
                newVxc,
                newVxc = ma4;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider11").hide();
    $("#container11").click(function () {
        $("#container11").hide("1000");
        $("#container12").hide("1000");
        $("#hider11").show("slow");
    });
    $("#hider11").click(function () {
        $("#container11").show("1000");
        $("#container12").show("1000");
        $("#hider11").hide("slow");
    });

    $('#hider12').highcharts({
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
            data: [mb4],
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
                newVxc,
                newVxc = mb4;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider12").hide();
    $("#container12").click(function () {
        $("#container11").hide("1000");
        $("#container12").hide("1000");
        $("#hider12").show("slow");
    });
    $("#hider12").click(function () {
        $("#container11").show("1000");
        $("#container12").show("1000");
        $("#hider12").hide("slow");
    });

    $('#hider13').highcharts({
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
            data: [ma5],
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
                newVxc,
                newVxc = ma5;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider13").hide();
    $("#container13").click(function () {
        $("#container13").hide("1000");
        $("#container14").hide("1000");
        $("#hider13").show("slow");
    });
    $("#hider13").click(function () {
        $("#container13").show("1000");
        $("#container14").show("1000");
        $("#hider13").hide("slow");
    });

    $('#hider14').highcharts({
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
            data: [mb5],
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
                newVxc,
                newVxc = mb5;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider14").hide();
    $("#container14").click(function () {
        $("#container13").hide("1000");
        $("#container14").hide("1000");
        $("#hider14").show("slow");
    });
    $("#hider14").click(function () {
        $("#container13").show("1000");
        $("#container14").show("1000");
        $("#hider14").hide("slow");
    });

    $('#hider15').highcharts({
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
            data: [ma6],
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
                newVxc,
                newVxc = ma6;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider15").hide();
    $("#container15").click(function () {
        $("#container15").hide("1000");
        $("#container16").hide("1000");
        $("#hider15").show("slow");
    });
    $("#hider15").click(function () {
        $("#container15").show("1000");
        $("#container16").show("1000");
        $("#hider15").hide("slow");
    });

    $('#hider16').highcharts({
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
            data: [mb6],
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
                newVxc,
                newVxc = mb6;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider16").hide();
    $("#container16").click(function () {
        $("#container15").hide("1000");
        $("#container16").hide("1000");
        $("#hider16").show("slow");
    });
    $("#hider16").click(function () {
        $("#container15").show("1000");
        $("#container16").show("1000");
        $("#hider16").hide("slow");
    });

    $('#hider17').highcharts({
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
            data: [ma7],
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
                newVxc,
                newVxc = ma7;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider17").hide();
    $("#container17").click(function () {
        $("#container17").hide("1000");
        $("#container18").hide("1000");
        $("#hider17").show("slow");
    });
    $("#hider17").click(function () {
        $("#container17").show("1000");
        $("#container18").show("1000");
        $("#hider17").hide("slow");
    });

    $('#hider18').highcharts({
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
            data: [mb7],
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
                newVxc,
                newVxc = mb7;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider18").hide();
    $("#container18").click(function () {
        $("#container17").hide("1000");
        $("#container18").hide("1000");
        $("#hider18").show("slow");
    });
    $("#hider18").click(function () {
        $("#container17").show("1000");
        $("#container18").show("1000");
        $("#hider18").hide("slow");
    });

    $('#hider19').highcharts({
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
            data: [ma8],
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
                newVxc,
                newVxc = ma8;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider19").hide();
    $("#container19").click(function () {
        $("#container19").hide("1000");
        $("#container20").hide("1000");
        $("#hider19").show("slow");
    });
    $("#hider19").click(function () {
        $("#container19").show("1000");
        $("#container20").show("1000");
        $("#hider19").hide("slow");
    });

    $('#hider20').highcharts({
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
            data: [mb8],
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
                newVxc,
                newVxc = mb8;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider20").hide();
    $("#container20").click(function () {
        $("#container19").hide("1000");
        $("#container20").hide("1000");
        $("#hider20").show("slow");
    });
    $("#hider20").click(function () {
        $("#container19").show("1000");
        $("#container20").show("1000");
        $("#hider20").hide("slow");
    });

    $('#hider21').highcharts({
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
            data: [ma9],
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
                newVxc,
                newVxc = ma9;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider21").hide();
    $("#container21").click(function () {
        $("#container21").hide("1000");
        $("#container22").hide("1000");
        $("#hider21").show("slow");
    });
    $("#hider21").click(function () {
        $("#container21").show("1000");
        $("#container22").show("1000");
        $("#hider21").hide("slow");
    });

    $('#hider22').highcharts({
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
            data: [mb9],
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
                newVxc,
                newVxc = mb9;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider22").hide();
    $("#container22").click(function () {
        $("#container21").hide("1000");
        $("#container22").hide("1000");
        $("#hider22").show("slow");
    });
    $("#hider22").click(function () {
        $("#container21").show("1000");
        $("#container22").show("1000");
        $("#hider22").hide("slow");
    });

    $('#hider23').highcharts({
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
            data: [ma10],
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
                newVxc,
                newVxc = ma10;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider23").hide();
    $("#container23").click(function () {
        $("#container23").hide("1000");
        $("#container24").hide("1000");
        $("#hider23").show("slow");
    });
    $("#hider23").click(function () {
        $("#container23").show("1000");
        $("#container24").show("1000");
        $("#hider23").hide("slow");
    });

    $('#hider24').highcharts({
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
            data: [mb10],
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
                newVxc,
                newVxc = mb10;
                point.update(newVxc);
            }, 60000);
        }
    });

    $("#hider24").hide();
    $("#container24").click(function () {
        $("#container23").hide("1000");
        $("#container24").hide("1000");
        $("#hider24").show("slow");
    });
    $("#hider24").click(function () {
        $("#container23").show("1000");
        $("#container24").show("1000");
        $("#hider24").hide("slow");
    });

    if (timeout == 0) {
        setTimeout(function () {
            set();
        },2000);
        timeout = 1;
    }
    //setInterval(function () {
    //    set();
    //}, 120000);



});

