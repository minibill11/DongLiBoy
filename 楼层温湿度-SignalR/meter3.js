$(function set() {
	    
		vb=parseFloat($.trim($("#temperature11").val()));	
		vc=parseFloat($.trim($("#humidness11").val()));
		vb2=parseFloat($.trim($("#temperature12").val()));	
		vc2=parseFloat($.trim($("#humidness12").val()));
		
		ma=parseFloat($.trim($("#temperature").val()));
		mb=parseFloat($.trim($("#humidness").val()));
		
		ma2=parseFloat($.trim($("#temperature2").val()));
		mb2=parseFloat($.trim($("#humidness2").val()));
		
		ma3=parseFloat($.trim($("#temperature3").val()));
		mb3=parseFloat($.trim($("#humidness3").val()));
		
		ma4=parseFloat($.trim($("#temperature4").val()));
		mb4=parseFloat($.trim($("#humidness4").val()));
		
		ma5=parseFloat($.trim($("#temperature5").val()));
		mb5=parseFloat($.trim($("#humidness5").val()));
		
		ma6=parseFloat($.trim($("#temperature6").val()));
		mb6=parseFloat($.trim($("#humidness6").val()));
		
		ma7=parseFloat($.trim($("#temperature7").val()));
		mb7=parseFloat($.trim($("#humidness7").val()));
		
		ma8=parseFloat($.trim($("#temperature8").val()));
		mb8=parseFloat($.trim($("#humidness8").val()));
		
		ma9=parseFloat($.trim($("#temperature9").val()));
		mb9=parseFloat($.trim($("#humidness9").val()));
		
		ma10=parseFloat($.trim($("#temperature10").val()));
		mb10=parseFloat($.trim($("#humidness10").val()));
		
		$("#container").highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' ℃'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
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
            style: { fontSize:'8px'  //字体
             } 
            }
        }]
       },
       function (chart) {
           if (!chart.renderer.forExport) {
               setInterval(function () {
                   var point = chart.series[0].points[0], 
                   newVal,
                   newVal = vb;
                   point.update(newVal);
               }, 60000);
           }
       });
		
	
    $('#container2').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
             } 
            
        }
            
        }]
        
    },
    function (chart) {
        if (!chart.renderer.forExport) {
            setInterval(function () {
                var point = chart.series[0].points[0], 
                newVac,
                newVac = vc;
                point.update(newVac);
            }, 60000);
        }
    });
    
    $('#container3').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' ℃'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
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
            style: { fontSize:'8px'  //字体
             } 
            
        }
            
        }]
    
    },
    function (chart) {
        if (!chart.renderer.forExport) {
            setInterval(function () {
                var point = chart.series[0].points[0], 
                newVaa,
                newVaa = vb2;
                point.update(newVaa);
            }, 60000);
        }
    });
    
    
    $('#container4').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
             } 
            
        }
            
        }]
        
    },
    function (chart) {
        if (!chart.renderer.forExport) {
            setInterval(function () {
                var point = chart.series[0].points[0], 
                newVab,
                newVab = vc2;
                point.update(newVab);
            }, 60000);
        }
    });
    
    $('#container5').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma2],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma3],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma4],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma5],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma6],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma7],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma8],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma9],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                   	
                    
                }]
        },
        series: [{
            name: '湿度',
            data: [ma10],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'8px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'8px'  //字体
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
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' ℃'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
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
            style: { fontSize:'11px'  //字体
             } 
          }
            
        }]
    
    },
    function (chart) {
        if (!chart.renderer.forExport) {
            setInterval(function () {
                var point = chart.series[0].points[0], 
                newVbc,
                newVbc = vb;
                point.update(newVbc);
            }, 60000);
        }
    });
    
    $("#hider").hide();
    $("#container").click(function(){
    	$("#container").hide("1000");
    	$("#container2").hide("1000");
    	$("#hider").show("slow");
    });
    $("#hider").click(function(){
    	$("#container").show("1000");
    	$("#container2").show("1000");
    	
    	$("#hider").hide("slow");
    });
   
    $('#hider2').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
             } 
        }
            
        }]
        
    },
    function (chart) {
        if (!chart.renderer.forExport) {
            setInterval(function () {
                var point = chart.series[0].points[0], 
                newVcc,
                newVcc = vc;
                point.update(newVcc);
            }, 60000);
        }
    });
    
    $("#hider2").hide();
    $("#container2").click(function(){
    	$("#container").hide("1000");
    	$("#container2").hide("1000");
    	$("#hider2").show("slow");
    });
    $("#hider2").click(function(){
    	$("#container").show("1000");
    	$("#container2").show("1000");
    	
    	$("#hider2").hide("slow");
    });
   
    
    $("#hider3").highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' ℃'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
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
            style: { fontSize:'11px'  //字体
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
    $("#container3").click(function(){
    	$("#container3").hide("1000");
    	$("#container4").hide("1000");
    	$("#hider3").show("slow");
    });
    $("#hider3").click(function(){
    	$("#container3").show("1000");
    	$("#container4").show("1000");
    	$("#hider3").hide("slow");
    });
    
    $('#hider4').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container4").click(function(){
    	$("#container3").hide("1000");
    	$("#container4").hide("1000");
    	$("#hider4").show("slow");
    });
    $("#hider4").click(function(){
    	$("#container3").show("1000");
    	$("#container4").show("1000");
    	
    	$("#hider4").hide("slow");
    });
    
    $("#hider5").highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' ℃'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
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
            style: { fontSize:'11px'  //字体
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
    $("#container5").click(function(){
    	$("#container5").hide("1000");
    	$("#container6").hide("1000");
    	$("#hider5").show("slow");
    });
    $("#hider5").click(function(){
    	$("#container5").show("1000");
    	$("#container6").show("1000");
    	$("#hider5").hide("slow");
    });
    
    $('#hider6').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container6").click(function(){
    	$("#container5").hide("1000");
    	$("#container6").hide("1000");
    	$("#hider6").show("slow");
    });
    $("#hider6").click(function(){
    	$("#container5").show("1000");
    	$("#container6").show("1000");
    	
    	$("#hider6").hide("slow");
    });
    
    $('#hider7').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma2],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container7").click(function(){
    	$("#container7").hide("1000");
    	$("#container8").hide("1000");
    	$("#hider7").show("slow");
    });
    $("#hider7").click(function(){
    	$("#container7").show("1000");
    	$("#container8").show("1000");
    	
    	$("#hider7").hide("slow");
    });
    
    $('#hider8').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container8").click(function(){
    	$("#container7").hide("1000");
    	$("#container8").hide("1000");
    	$("#hider8").show("slow");
    });
    $("#hider8").click(function(){
    	$("#container7").show("1000");
    	$("#container8").show("1000");
    	
    	$("#hider8").hide("slow");
    });
    
    $('#hider9').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma3],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container9").click(function(){
    	$("#container9").hide("1000");
    	$("#container10").hide("1000");
    	$("#hider9").show("slow");
    });
    $("#hider9").click(function(){
    	$("#container9").show("1000");
    	$("#container10").show("1000");
    	$("#hider9").hide("slow");
    });
    
    $('#hider10').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container10").click(function(){
    	$("#container9").hide("1000");
    	$("#container10").hide("1000");
    	$("#hider10").show("slow");
    });
    $("#hider10").click(function(){
    	$("#container9").show("1000");
    	$("#container10").show("1000");
    	$("#hider10").hide("slow");
    });
    
    $('#hider11').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma4],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container11").click(function(){
    	$("#container11").hide("1000");
    	$("#container12").hide("1000");
    	$("#hider11").show("slow");
    });
    $("#hider11").click(function(){
    	$("#container11").show("1000");
    	$("#container12").show("1000");
      	$("#hider11").hide("slow");
    });
    
    $('#hider12').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container12").click(function(){
    	$("#container11").hide("1000");
    	$("#container12").hide("1000");
    	$("#hider12").show("slow");
    });
    $("#hider12").click(function(){
    	$("#container11").show("1000");
    	$("#container12").show("1000");
    	$("#hider12").hide("slow");
    });
    
    $('#hider13').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma5],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container13").click(function(){
    	$("#container13").hide("1000");
    	$("#container14").hide("1000");
    	$("#hider13").show("slow");
    });
    $("#hider13").click(function(){
    	$("#container13").show("1000");
    	$("#container14").show("1000");
    	$("#hider13").hide("slow");
    });
    
    $('#hider14').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container14").click(function(){
    	$("#container13").hide("1000");
    	$("#container14").hide("1000");
    	$("#hider14").show("slow");
    });
    $("#hider14").click(function(){
    	$("#container13").show("1000");
    	$("#container14").show("1000");
    	$("#hider14").hide("slow");
    });
    
    $('#hider15').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma6],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container15").click(function(){
    	$("#container15").hide("1000");
    	$("#container16").hide("1000");
    	$("#hider15").show("slow");
    });
    $("#hider15").click(function(){
    	$("#container15").show("1000");
    	$("#container16").show("1000");
    	$("#hider15").hide("slow");
    });
    
    $('#hider16').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container16").click(function(){
    	$("#container15").hide("1000");
    	$("#container16").hide("1000");
    	$("#hider16").show("slow");
    });
    $("#hider16").click(function(){
    	$("#container15").show("1000");
    	$("#container16").show("1000");
    	$("#hider16").hide("slow");
    });
    
    $('#hider17').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma7],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container17").click(function(){
    	$("#container17").hide("1000");
    	$("#container18").hide("1000");
    	$("#hider17").show("slow");
    });
    $("#hider17").click(function(){
    	$("#container17").show("1000");
    	$("#container18").show("1000");
    	$("#hider17").hide("slow");
    });
    
    $('#hider18').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container18").click(function(){
    	$("#container17").hide("1000");
    	$("#container18").hide("1000");
    	$("#hider18").show("slow");
    });
    $("#hider18").click(function(){
    	$("#container17").show("1000");
    	$("#container18").show("1000");
    	$("#hider18").hide("slow");
    });
    
    $('#hider19').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma8],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container19").click(function(){
    	$("#container19").hide("1000");
    	$("#container20").hide("1000");
    	$("#hider19").show("slow");
    });
    $("#hider19").click(function(){
    	$("#container19").show("1000");
    	$("#container20").show("1000");
    	$("#hider19").hide("slow");
    });
    
    $('#hider20').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container20").click(function(){
    	$("#container19").hide("1000");
    	$("#container20").hide("1000");
    	$("#hider20").show("slow");
    });
    $("#hider20").click(function(){
    	$("#container19").show("1000");
    	$("#container20").show("1000");
    	$("#hider20").hide("slow");
    });
    
    $('#hider21').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma9],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container21").click(function(){
    	$("#container21").hide("1000");
    	$("#container22").hide("1000");
    	$("#hider21").show("slow");
    });
    $("#hider21").click(function(){
    	$("#container21").show("1000");
    	$("#container22").show("1000");
    	$("#hider21").hide("slow");
    });
    
    $('#hider22').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container22").click(function(){
    	$("#container21").hide("1000");
    	$("#container22").hide("1000");
    	$("#hider22").show("slow");
    });
    $("#hider22").click(function(){
    	$("#container21").show("1000");
    	$("#container22").show("1000");
    	$("#hider22").hide("slow");
    });
    
    $('#hider23').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
                    to: 100,
                   	color: '#DF5353', // red
                 }]
        },
        series: [{
            name: '湿度',
            data: [ma10],
            tooltip: {
                valueSuffix: ' %RH'
            },
            dataLabels: {
           /* enabled: true, */
            color: '#19a0f5',
            style: { fontSize:'11px'  //字体
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
    $("#container23").click(function(){
    	$("#container23").hide("1000");
    	$("#container24").hide("1000");
    	$("#hider23").show("slow");
    });
    $("#hider23").click(function(){
    	$("#container23").show("1000");
    	$("#container24").show("1000");
    	$("#hider23").hide("slow");
    });
    
    $('#hider24').highcharts({
        chart: {
            type: 'gauge',
            backgroundColor: 'rgba(255, 255, 255, 0)',
            plotBackgroundColor:null,
            plotBackgroundImage: null,
            plotBorderWidth: null,
            plotShadow: false,
            plotBorderColor : null
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
           		step: 2,
                rotation: 'auto',
                style: {
                            color: '#19a0f5',//颜色
                            fontSize:'11px'  //字体
                        }
            },
            title: {
                text: ' %RH'
            },
            plotBands: [
                {
                    from: 0,
                    to: 20,
                    color: '#DF5353', // green
                    
                 },{
                    from: 20,
                    to: 37,
                    color: '#55BF3B' ,// green
                    
                }, {
                    from: 37,
                    to: 40,
                    color: '#DDDF0D', // yellow
                    
                }, {
                    from: 40,
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
            style: { fontSize:'11px'  //字体
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
    $("#container24").click(function(){
    	$("#container23").hide("1000");
    	$("#container24").hide("1000");
    	$("#hider24").show("slow");
    });
    $("#hider24").click(function(){
    	$("#container23").show("1000");
    	$("#container24").show("1000");
    	$("#hider24").hide("slow");
    });
    
    function myrefresh(){
    	var timestamp = Date.parse(new Date());
    	var vt = $("#temperature11").val()+"";
    	var vm = $("#humidness11").val()+"";
    	var vo = $("#temperature12").val()+"";
		var vu = $("#humidness12").val()+"";
		var n1 = $("#temperature").val()+"";
    	var n1a = $("#humidness").val()+"";
    	var n2 = $("#temperature2").val()+"";
		var n2a = $("#humidness2").val()+"";
		var n3 = $("#temperature3").val()+"";
    	var n3a = $("#humidness3").val()+"";
    	var n4 = $("#temperature4").val()+"";
		var n4a = $("#humidness4").val()+"";
		var n5 = $("#temperature5").val()+"";
    	var n5a = $("#humidness5").val()+"";
    	var n6 = $("#temperature6").val()+"";
		var n6a = $("#humidness6").val()+"";
		var n7 = $("#temperature7").val()+"";
    	var n7a = $("#humidness7").val()+"";
    	var n8 = $("#temperature8").val()+"";
		var n8a = $("#humidness8").val()+"";
		var n9 = $("#temperature9").val()+"";
    	var n9a = $("#humidness9").val()+"";
    	var n10 = $("#temperature10").val()+"";
		var n10a = $("#humidness10").val()+"";
    	var url = '/JianHeSysTest/pricnside/five_floor.do';
    	$.ajax({
    	async : true,//同步
    	type : 'post',
    	url : url,
    	data : {vt:vt,vm:vm,vo:vo,vu:vu,n1,n1a,n2,n2a,n3,n3a,n4,n4a,n5,n5a,n6,n6a,n7,n7a,n8,n8a,n9,n9a,n10,n10a},
    	dataType : 'json',
    	timeout:3000,
    	success:function(result){
    		if(result != "" && result !=null){
    			var myobj=eval(result);
    			vd=parseFloat(myobj[0]);
    			vz=parseFloat(myobj[1]);
    			The_vo=parseFloat(myobj[2]);
    			The_vu=parseFloat(myobj[3]);
    			the_time=myobj[6];
    			the_time2=myobj[7];
    			the_s1=parseInt(myobj[4]);
    			the_s2=parseInt(myobj[5]);
    			nl=parseFloat(myobj[9]);
    			nl2=parseFloat(myobj[10]);
    			nl3=parseFloat(myobj[11]);
    			nl4=parseFloat(myobj[12]);
    			nl5=parseFloat(myobj[13]);
    			nl6=parseFloat(myobj[14]);
    			nl7=parseFloat(myobj[15]);
    			nl8=parseFloat(myobj[16]);
    			nl9=parseFloat(myobj[17]);
    			nl10=parseFloat(myobj[18]);
    			nl11=parseFloat(myobj[19]);
    			nl12=parseFloat(myobj[20]);
    			nl13=parseFloat(myobj[21]);
    			nl14=parseFloat(myobj[22]);
    			nl15=parseFloat(myobj[23]);
    			nl16=parseFloat(myobj[24]);
    			nl17=parseFloat(myobj[25]);
    			nl18=parseFloat(myobj[26]);
    			nl19=parseFloat(myobj[27]);
    			nl20=parseFloat(myobj[28]);
    			the_ti=myobj[39];
    			the_ti2=myobj[40];
    			the_ti3=myobj[41];
    			the_ti4=myobj[42];
    			the_ti5=myobj[43];
    			the_ti6=myobj[44];
    			the_ti7=myobj[45];
    			the_ti8=myobj[46];
    			the_ti9=myobj[47];
    			the_ti10=myobj[48];
    			the_S1=parseInt(myobj[29]);
    			the_S2=parseInt(myobj[30]);
    			the_S3=parseInt(myobj[31]);
    			the_S4=parseInt(myobj[32]);
    			the_S5=parseInt(myobj[33]);
    			the_S6=parseInt(myobj[34]);
    			the_S7=parseInt(myobj[35]);
    			the_S8=parseInt(myobj[36]);
    			the_S9=parseInt(myobj[37]);
    			the_S10=parseInt(myobj[38]);
    			if(the_s1==0){
	    			$("#statu11").val("正常");
	    			$("#statu11").css("backgroundColor","green");
	    			}else{
	    			$("#statu11").val("警报");	
	    			$("#statu11").css("backgroundColor","red");
	    			}
    			if(the_s2==0){
        			$("#statu12").val("正常");
        			$("#statu12").css("backgroundColor","green");
        			}else{
        			$("#statu12").val("警报");	
        			$("#statu12").css("backgroundColor","red");
        			}
    			if(the_S1==0){
	    			$("#statu").val("正常");
	    			$("#statu").css("backgroundColor","green");
	    			}else{
	    			$("#statu").val("警报");	
	    			$("#statu").css("backgroundColor","red");
	    			}
    			if(the_S2==0){
        			$("#statu2").val("正常");
        			$("#statu2").css("backgroundColor","green");
        			}else{
        			$("#statu2").val("警报");	
        			$("#statu2").css("backgroundColor","red");
        			}
    			if(the_S3==0){
	    			$("#statu3").val("正常");
	    			$("#statu3").css("backgroundColor","green");
	    			}else{
	    			$("#statu3").val("警报");	
	    			$("#statu3").css("backgroundColor","red");
	    			}
    			if(the_S4==0){
        			$("#statu4").val("正常");
        			$("#statu4").css("backgroundColor","green");
        			}else{
        			$("#statu4").val("警报");	
        			$("#statu4").css("backgroundColor","red");
        			}
    			if(the_S5==0){
	    			$("#statu5").val("正常");
	    			$("#statu5").css("backgroundColor","green");
	    			}else{
	    			$("#statu5").val("警报");	
	    			$("#statu5").css("backgroundColor","red");
	    			}
    			if(the_S6==0){
        			$("#statu6").val("正常");
        			$("#statu6").css("backgroundColor","green");
        			}else{
        			$("#statu6").val("警报");	
        			$("#statu6").css("backgroundColor","red");
        			}
    			if(the_S7==0){
	    			$("#statu7").val("正常");
	    			$("#statu7").css("backgroundColor","green");
	    			}else{
	    			$("#statu7").val("警报");	
	    			$("#statu7").css("backgroundColor","red");
	    			}
    			if(the_S8==0){
        			$("#statu8").val("正常");
        			$("#statu8").css("backgroundColor","green");
        			}else{
        			$("#statu8").val("警报");	
        			$("#statu8").css("backgroundColor","red");
        			}
    			if(the_S9==0){
	    			$("#statu9").val("正常");
	    			$("#statu9").css("backgroundColor","green");
	    			}else{
	    			$("#statu9").val("警报");	
	    			$("#statu9").css("backgroundColor","red");
	    			}
    			if(the_S10==0){
        			$("#statu10").val("正常");
        			$("#statu10").css("backgroundColor","green");
        			}else{
        			$("#statu10").val("警报");	
        			$("#statu10").css("backgroundColor","red");
        			}
    			
    			var svl = Date.parse(the_time);
    			var svl2 = Date.parse(the_time2);
    			var svt = Date.parse(the_ti);
    			var svt2 = Date.parse(the_ti2);
    			var svt3 = Date.parse(the_ti3);
    			var svt4 = Date.parse(the_ti4);
    			var svt5 = Date.parse(the_ti5);
    			var svt6 = Date.parse(the_ti6);
    			var svt7 = Date.parse(the_ti7);
    			var svt8 = Date.parse(the_ti8);
    			var svt9 = Date.parse(the_ti9);
    			var svt10 = Date.parse(the_ti10);
    			if(timestamp-svl<120000){
    			$('#temperature11').val(vd);
    			vb=parseFloat(vd);
    			$("#humidness11").val(vz);
    			vc=parseFloat(vz);
    			}else{
    				$('#temperature11').val('#');
    				vb=parseFloat(0);
    				$("#humidness11").val('#');
    				vc=parseFloat(0);
    			}
    			if(timestamp-svl2<120000){
    				$('#temperature12').val(The_vo);
        			vb2=parseFloat(The_vo);
        			$("#humidness12").val(The_vu);
        			vc2=parseFloat(The_vu);
    			}else{
    				$('#temperature12').val('#');
    				vb2==parseFloat(0);
    				$("#humidness12").val('#');
    				vc2=parseFloat(0);
    			}
    			if(timestamp-svt<120000){
    				$('#temperature').val(n1);
        			ma=parseFloat(n1);
        			$("#humidness").val(n1a);
        			mb=parseFloat(n1a);
    			}else{
    				$('#temperature').val('#');
    				ma==parseFloat(0);
    				$("#humidness").val('#');
    				mb=parseFloat(0);
    			}
    			if(timestamp-svt2<120000){
    				$('#temperature2').val(n2);
        			ma=parseFloat(n2);
        			$("#humidness2").val(n2a);
        			mb=parseFloat(n2a);
    			}else{
    				$('#temperature2').val('#');
    				ma==parseFloat(0);
    				$("#humidness2").val('#');
    				mb=parseFloat(0);
    			}
    			if(timestamp-svt3<120000){
    				$('#temperature3').val(n3);
        			ma=parseFloat(n3);
        			$("#humidness3").val(n3a);
        			mb=parseFloat(n3a);
    			}else{
    				$('#temperature3').val('#');
    				ma==parseFloat(0);
    				$("#humidness3").val('#');
    				mb=parseFloat(0);
    			}
    			if(timestamp-svt4<120000){
    				$('#temperature4').val(n4);
        			ma=parseFloat(n4);
        			$("#humidness4").val(n4a);
        			mb=parseFloat(n4a);
    			}else{
    				$('#temperature4').val('#');
    				ma==parseFloat(0);
    				$("#humidness4").val('#');
    				mb=parseFloat(0);
    			}
    			if(timestamp-svt5<120000){
    				$('#temperature5').val(n5);
        			ma=parseFloat(n5);
        			$("#humidness5").val(n5a);
        			mb=parseFloat(n5a);
    			}else{
    				$('#temperature5').val('#');
    				ma==parseFloat(0);
    				$("#humidness5").val('#');
    				mb=parseFloat(0);
    			}
    		}else{
    		alert("参数错误");	
    		}
    	},
    	error:function(){
    		//alert("请求错误！");
    	},
    	});
    }	
    	setInterval(function() { 
    		myrefresh();}, 60000);
});

	