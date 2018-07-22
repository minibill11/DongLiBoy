	
	function myrefresh(){
		 var j = document.getElementById("run").value;
		 /*var k = document.getElementById("testre").value;
		 var k2 = document.getElementById("testre2").value;*/
		 var js = document.getElementById("run2").value;
		 /*var kaa = document.getElementById("testre3").value;
		 var kee = document.getElementById("testre4").value;*/
		 var qs = document.getElementById("run3").value;
		 /*var qaa = document.getElementById("testre5").value;
		 var qee = document.getElementById("testre6").value;*/
		 var grm = $("#rm").val();
		 var rm = parseInt(grm);
		 var ghd4 = $("#hd4").val();
		 var hd4 = parseInt(ghd4);
		 var ghd3 = $("#hd3").val();
		 var hd3 = parseInt(ghd3);
		 var gdry2 = $("#dry2").val();
		 var dry2 = parseInt(gdry2);
		 var gdry = $("#dry").val();
		 var dry = parseInt(gdry);
		 var gac3 = $("#acpl3").val();
		 var acpl3 = parseInt(gac3);
		 var gac2 = $("#acpl2").val();
		 var acpl2 = parseInt(gac2);
		 var gac = $("#acpl").val();
		 var acpl = parseInt(gac);
		 var gna2 = $("#na2").val();
		 var na2 = parseInt(gna2);
		 var gna = $("#na").val();
		 var na = parseInt(gna);
		 var array = new Array();
		 var array2 = new Array();
		 var arrayt = new Array();
		 var arrayt2 = new Array();
		 var arrayt3 = new Array();
		 var arraydr= new Array();
		 var arraydr2= new Array();
		 var arrayh3= new Array();
		 var arrayh4= new Array();
		 var arrayrm= new Array();
		 if(j=='正常'){
			document.getElementById("run").style.backgroundColor="#00FA9A";
			/*document.getElementById("trouble_removal").style.backgroundColor="#00FA9A";
			document.getElementById("trouble_removal2").style.backgroundColor="#00FA9A";*/
		}else{
			document.getElementById("run").style.backgroundColor="yellow";
			/*if(k==1){
			document.getElementById("trouble_removal").style.backgroundColor="red";
			document.getElementById("trouble_removal2").style.backgroundColor="#00FA9A";
			}
			if(k2==2){
			document.getElementById("trouble_removal2").style.backgroundColor="red";
			document.getElementById("trouble_removal").style.backgroundColor="#00FA9A";
			}*/
		}
		 if(js=='正常'){
			document.getElementById("run2").style.backgroundColor="#00FA9A";
			/*document.getElementById("trouble_removal3").style.backgroundColor="#00FA9A";
			document.getElementById("trouble_removal4").style.backgroundColor="#00FA9A";*/
		}else{
			document.getElementById("run2").style.backgroundColor="yellow";
			/*if(kaa==1){
			document.getElementById("trouble_removal3").style.backgroundColor="red";
			document.getElementById("trouble_removal4").style.backgroundColor="#00FA9A";
			}
			if(kee==2){
			document.getElementById("trouble_removal4").style.backgroundColor="red";
			document.getElementById("trouble_removal3").style.backgroundColor="#00FA9A";
			}*/
		}
		if(qs=='正常'){
			document.getElementById("run3").style.backgroundColor="#00FA9A";
			/*document.getElementById("trouble_removal5").style.backgroundColor="#00FA9A";
			document.getElementById("trouble_removal6").style.backgroundColor="#00FA9A";*/
		}else{
			document.getElementById("run3").style.backgroundColor="yellow";
			/*if(qaa==1){
			document.getElementById("trouble_removal5").style.backgroundColor="red";
			document.getElementById("trouble_removal6").style.backgroundColor="#00FA9A";
			}
			if(qee==2){
			document.getElementById("trouble_removal6").style.backgroundColor="red";
			document.getElementById("trouble_removal5").style.backgroundColor="#00FA9A";
			}*/
		}
		
		 rls1 = $("#runlist").val();
		 rls2 = $("#runlist2").val();
		 rls3 = $("#runlist3").val();
		 
		 apl1 = $("#acplist").val();
		 apl2 = $("#acplist2").val();
		 apl3 = $("#acplist3").val();
		 
		 dyl1 = $("#drylist").val();
		 dyl2 = $("#drylist2").val();
		 
		 dhl1 = $("#drchlist").val();
		 dhl2 = $("#drchlist2").val();
		 dhl3 = $("#drchlist3").val();
		 dhl4 = $("#drchlist4").val();
		  
		 hpl1 = $("#hdplist").val();
		 hpl2 = $("#hdplist2").val();
		 th = $("#thlist").val();
		 var url='/JianHeSysTest/new/prcinside1.do';
		 $.ajax({
			 async : true,
			 type : 'post',
		     data : {rls1:rls1,rls2:rls2,rls3:rls3,apl1:apl1,apl2:apl2,apl3:apl3,dyl1:dyl1,dhl2:dhl2,dhl1:dhl1,dhl2:dhl2,dhl3:dhl3,dhl4:dhl4,hpl1:hpl1,hpl2:hpl2,th:th},
		     dataType : 'json',
		     cache : false,
		     url : url,
		     timeout:1000,
		     success:function(map){
		    	 var g;
		    	 var a;
		    	 var b;
		    	 var c;
		    	 var d;
		    	 var e;
		    	 var f;
		    	 var g;
		    	 var h;
		    	 var I;
		    	 var j;
		    	 var k;
		    	 var l;
		    	 var z;
		    	 var x;
		    	 var J;
		    	 var m;
		    	var arrmap=eval(map);
		    	var cval=$("#sdate").val();
		    	var cval2=$("#sdate2").val();
		    	var cval3=$("#sdate3").val();
		    	var cval6=$("#sdate4").val();
		    	var cval4=$("#sdate5").val();
		    	var cval5=$("#sdate6").val();
		    	var cval7=$("#sdate7").val();
		    	var cval8=$("#sdate8").val();
		    	var cval9=$("#sdate9").val();
		    	var cval10=$("#sdate10").val();
		    	var cval11=$("#sdate11").val();
		    	var cval12=$("#sdate12").val();
		    	var cval13=$("#sdate13").val();
		    	var cval14=$("#sdate14").val();
		    	var cval15=$("#thlist2").val();
		    	for(var i=0;i<arrmap.length;i++){
		    		var am= arrmap[i];
		    		var ar=eval(am);
		    		for(var key in ar){
		    			J=ar[key];
		    			g = ar['g'];
		    			a = ar['a'];
		    			b = ar['b'];
		    			c = ar['c'];
		    			d = ar['d'];
		    			e = ar['e'];
		    			f = ar['f'];
		    			h = ar['h'];
		    			I = ar['i'];
		    			j = ar['j'];
		    			k = ar['k'];
		    			l = ar['l'];
		    			z = ar['z'];
		    			x = ar['x'];
		    			m = ar['m'];
		    		}
		    		} 
		    	 	var aa=eval(a);
		    	 	var ab=eval(b);
		    	 	var ac=eval(c);
		    	 	var ad=eval(d);
		    	 	var ae=eval(e);
		    	 	var af=eval(f);
		    	 	var ag=eval(g);
		    	 	var ah=eval(h);
		    	 	/*var ai=eval(I);
		    	 	var aj=eval(j);
		    	 	var ak=eval(k);
		    	 	var al=eval(l);*/
		    	 	var az=eval(z);
		    	 	var ax=eval(x);
		    	 	var am=eval(m);
		    	 	/*var text = $("#testre").val();*/
	    	 		/*Pre7 = aj[0].pressure;
	    	 		Tem7 = aj[0].temperature;
	    	 		Hum7 = aj[0].humidity;*/
	    	 		
	    	 		run = aa[1].status;
	    	 		/*trbval = aa[1].slight;
	    	 		trbval2 = aa[1].severity;*/
	    	 		disc = aa[1].pressure;
	    	 		Temp = aa[1].temperature;
	    	 		cu = aa[1].current_u;
	    	 		cv = aa[1].current_v;
	    	 		cw = aa[1].current_w;
	    	 		
	    	 		run2 = ab[1].status;
	    	 		/*trbval3 = ab[1].slight;
	    	 		trbval4 = ab[1].severity;*/
	    	 		disc2 = ab[1].pressure;
	    	 		Temp2 = ab[1].temperature;
	    	 		cu2 = ab[1].current_u;
	    	 		cv2 = ab[1].current_v;
	    	 		cw2 = ab[1].current_w;
	    	 		
	    	 		run3 = ac[1].status;
	    	 		/*trbval5 = ac[1].slight;
	    	 		trbval6 = ac[1].severity;*/
	    	 		disc3 = ac[1].pressure;
	    	 		Temp3 = ac[1].temperature;
	    	 		cu3 = ac[1].current_u;
	    	 		cv3 = ac[1].current_v;
	    	 		cw3 = ac[1].current_w;
	    	 		
	    	 		Pre = ad[0].pressure;
	    	 		Tem = ad[0].temperature;
	    	 		Hum = ad[0].humidity;
	    	 		
	    	 		Pre2 = ae[0].pressure;
	    	 		Tem2 = ae[0].temperature;
	    	 		Hum2 = ae[0].humidity;
	    	 		
	    	 		Pre3 = af[0].pressure;
	    	 		Tem3 = af[0].temperature;
	    	 		Hum3 = af[0].humidity;
	    	 		
	    	 		Pre4 = ag[0].pressure;
	    	 		Tem4 = ag[0].temperature;
	    	 		Hum4 = ag[0].humidity;
	    	 		
	    	 		Pre5 = ah[0].pressure;
	    	 		Tem5 = ah[0].temperature;
	    	 		Hum5 = ah[0].humidity;

	    	 		/*Pre6 = ai[0].pressure;
	    	 		Tem6 = ai[0].temperature;
	    	 		Hum6 = ai[0].humidity;*/
	    	 		
	    	 		/*Pre8 = ak[0].pressure;
	    	 		Tem8 = ak[0].temperature;
	    	 		Hum8 = ak[0].humidity;*/
	    	 		
	    	 		/*Pre9 = al[0].pressure;
	    	 		Tem9 = al[0].temperature;
	    	 		Hum9 = al[0].humidity;*/
	    	 		
	    	 		Pre10 = az[0].pressure;
	    	 		Tem10 = az[0].temperature;
	    	 		Hum10 = az[0].humidity;
	    	 		
	    	 		Pre12 = ax[0].pressure;
	    	 		Tem12 = ax[0].temperature;
	    	 		Hum12 = ax[0].humidity;
	    	 		
	    	 		Tem13 = am[0].temperature;
	    	 		Hum13 = am[0].humidity;
	    	 		
	    	 		for(var i=0;i<aa.length;i++){
	    	 			if(aa[i].status!=1){
	    	 				na=na+1;
	    	 				array.push(aa[i].id);
	    	 		}}
	    	 		$("#na").val(na);
	    	 		if(na==0){
    	 				$("#aircompA").show();
    	 				$("#aircompA2").hide();
    	 			}else{
    	 				$("#aircompA2").show();
    	 				$("#aircompA").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<ab.length;i++){
	    	 			if(ab[i].status!=1){
	    	 				na2=na2+1;
	    	 				array2.push(ab[i].id);
	    	 		}}
	    	 		$("#na2").val(na2);
	    	 		if(na2==0){
    	 				$("#aircompB").show();
    	 				$("#aircompB2").hide();
    	 			}else{
    	 				$("#aircompB2").show();
    	 				$("#aircompB").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<ad.length;i++){
	    	 			if(ad[i].statu!=0){
	    	 				acpl=acpl+1;
	    	 				arrayt.push(ab[i].id);
	    	 		}}
	    	 		$("#acpl").val(acpl);
	    	 		if(acpl==0){
    	 				$("#acpListA").show();
    	 				$("#acpListA2").hide();
    	 			}else{
    	 				$("#acpListA2").show();
    	 				$("#acpListA").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<ae.length;i++){
	    	 			if(ae[i].statu!=0){
	    	 				acpl2=acpl2+1;
	    	 		}}
	    	 		$("#acpl2").val(acpl2);
	    	 		if(acpl2==0){
    	 				$("#acpListB").show();
    	 				$("#acpListB2").hide();
    	 			}else{
    	 				$("#acpListB2").show();
    	 				$("#acpListB").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<af.length;i++){
	    	 			if(af[i].statu!=0){
	    	 				acpl3=acpl3+1;
	    	 		}}
	    	 		$("#acpl3").val(acpl3);
	    	 		if(acpl3==0){
    	 				$("#acpListC").show();
    	 				$("#acpListC2").hide();
    	 			}else{
    	 				$("#acpListC2").show();
    	 				$("#acpListC").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<ag.length;i++){
	    	 			if(ag[i].statu!=0){
	    	 				dry=dry+1;
	    	 		}}
	    	 		$("#dry").val(dry);
	    	 		if(dry==0){
    	 				$("#dryerA").show();
    	 				$("#dryerA2").hide();
    	 			}else{
    	 				$("#dryerA2").show();
    	 				$("#dryerA").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<ah.length;i++){
	    	 			if(ah[i].statu!=0){
	    	 				dry2=dry2+1;
	    	 		}}
	    	 		$("#dry2").val(dry2);
	    	 		if(dry2==0){
    	 				$("#dryerB").show();
    	 				$("#dryerB2").hide();
    	 			}else{
    	 				$("#dryerB2").show();
    	 				$("#dryerB").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<az.length;i++){
	    	 			if(az[i].statu!=0){
	    	 				hd3=hd3+1;
	    	 		}}
	    	 		$("#hd3").val(hd3);
	    	 		if(hd3==0){
    	 				$("#headerpipe3in").show();
    	 				$("#headerpipe3in2").hide();
    	 			}else{
    	 				$("#headerpipe3in2").show();
    	 				$("#headerpipe3in").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<ax.length;i++){
	    	 			if(ax[i].statu!=0){
	    	 				hd4=hd4+1;
	    	 		}}
	    	 		$("#hd4").val(hd4);
	    	 		if(hd4==0){
    	 				$("#headerpipe4in").show();
    	 				$("#headerpipe4in2").hide();
    	 			}else{
    	 				$("#headerpipe4in2").show();
    	 				$("#headerpipe4in").hide();
    	 			}
	    	 		
	    	 		for(var i=0;i<am.length;i++){
	    	 			if(am[i].statu!=0){
	    	 				rm=rm+1;
	    	 			}}
	    	 			$("#rm").val(rm);
	    	 			if(rm==0){
	    	 				$("#room").show();
	    	 				$("#room2").hide();
	    	 			}else{
	    	 				$("#room2").show();
	    	 				$("#room").hide();
	    	 			}
	    	 		
	    	 		/*if(Tem7==-40){
		    	 		$("#Temperature7").val(0);
		    	 	}else{
		    	 		$("#Temperature7").val(Tem7);
		    	 	}
		    	 	if(Hum7==5){
		    	 		$("#Humidity7").val(0);
		    	 	}else{
		    	 		$("#Humidity7").val(Hum7);
		    	 	}
		    	 	$("#Pressure7").val(Pre7);*/
		    	 	
		    	 	if(Temp==-40){
		    	 		$("#Temper").val(0);
		    	 	}else{
		    	 		$("#Temper").val(parseFloat(Temp));
		    	 	}
		    	 	$("#run").val(run==1?"正常":"停机");
		    	 	/*$("#testre").val(trbval);
		    	 	$("#testre2").val(trbval2);*/
		    	 	$("#discharge").val(parseFloat(disc));
		    	 	$("#current_u").val(parseFloat(cu));
		    	 	/*$("#current_v").val(parseFloat(cv));
		    	 	$("#current_w").val(parseFloat(cw));*/
		    	 	
		    	 	if(Temp2==-40){
		    	 		$("#Temper2").val(0);
		    	 	}else{
		    	 		$("#Temper2").val(parseFloat(Temp2));
		    	 	}
		    	 	$("#run2").val(run2==1?"正常":"停机");
		    	 	/*$("#testre3").val(trbval3);
		    	 	$("#testre4").val(trbval4);*/
		    	 	$("#discharge2").val(parseFloat(disc2));
		    	 	$("#current_u2").val(parseFloat(cu2));
		    	 	/*$("#current_v2").val(parseFloat(cv2));
		    	 	$("#current_w2").val(parseFloat(cw2));*/
		    	 	
		    	 	if(Temp3==-40){
		    	 		$("#Temper3").val(0);
		    	 	}else{
		    	 		$("#Temper3").val(Temp3);
		    	 	}
		    	 	$("#run3").val(run3==1?"正常":"停机");
		    	 	/*$("#testre5").val(trbval5);
		    	 	$("#testre6").val(trbval6);*/
		    	 	$("#discharge3").val(parseFloat(disc3));
		    	 	$("#current_u3").val(cu3);
		    	 	/*$("#current_v3").val(cv3);
		    	 	$("#current_w3").val(cw3);*/
		    	 	
		    	 	if(Tem2==-40){
		    	 		$("#Temperature").val(0);
		    	 	}else{
		    	 		$("#Temperature").val(Tem2);
		    	 	}
		    	 	if(Hum2==5){
		    	 		$("#Humidity").val(0);
		    	 	}else{
		    	 		$("#Humidity").val(Hum2);
		    	 	}
		    	 	$("#Pressure").val(Pre2);
		    	 	
		    	 	if(Tem3==-40){
		    	 		$("#Temperature2").val(0);
		    	 	}else{
		    	 		$("#Temperature2").val(Tem3);
		    	 	}
		    	 	if(Hum3==5){
		    	 		$("#Humidity2").val(0);
		    	 	}else{
		    	 		$("#Humidity2").val(Hum3);
		    	 	}
		    	 	$("#Pressure2").val(Pre3);
		    	 	
		    	 	if(Tem==-40){
		    	 		$("#Temperature3").val(0);
		    	 	}else{
		    	 		$("#Temperature3").val(Tem);
		    	 	}
		    	 	if(Hum==5){
		    	 		$("#Humidity3").val(0);
		    	 	}else{
		    	 		$("#Humidity3").val(Hum);
		    	 	}
		    	 	$("#Pressure3").val(Pre);
		    	 	
		    	 	if(Tem4==-40){
		    	 		$("#Temperature4").val(0);
		    	 	}else{
		    	 		$("#Temperature4").val(Tem4);
		    	 	}
		    	 	if(Hum4==5){
		    	 		$("#Humidity4").val(0);
		    	 	}else{
		    	 		$("#Humidity4").val(Hum4);
		    	 	}
		    	 	$("#Pressure4").val(Pre4);
		    	 	
		    	 	if(Tem5==-40){
		    	 		$("#Temperature5").val(0);
		    	 	}else{
		    	 		$("#Temperature5").val(Tem5);
		    	 	}
		    	 	if(Hum5==5){
		    	 		$("#Humidity5").val(0);
		    	 	}else{
		    	 		$("#Humidity5").val(Hum5);
		    	 	}
		    	 	$("#Pressure5").val(Pre5);
		    	 	
		    	 	/*if(Tem6==-40){
		    	 		$("#Temperature6").val(0);
		    	 	}else{
		    	 		$("#Temperature6").val(Tem6);
		    	 	}
		    	 	if(Hum6==5){
		    	 		$("#Humidity6").val(0);
		    	 	}else{
		    	 		$("#Humidity6").val(Hum6);
		    	 	}
		    	 	$("#Pressure6").val(Pre6);*/
		    	 	
		    	 	/*if(Tem8==-40){
		    	 		$("#Temperature8").val(0);
		    	 	}else{
		    	 		$("#Temperature8").val(Tem8);
		    	 	}
		    	 	if(Hum8==5){
		    	 		$("#Humidity8").val(0);
		    	 	}else{
		    	 		$("#Humidity8").val(Hum8);
		    	 	}
		    	 	$("#Pressure8").val(Pre8);
		    	 	
		    	 	if(Tem9==-40){
		    	 		$("#Temperature9").val(0);
		    	 	}else{
		    	 		$("#Temperature9").val(Tem9);
		    	 	}
		    	 	if(Hum9==5){
		    	 		$("#Humidity9").val(0);
		    	 	}else{
		    	 		$("#Humidity9").val(Hum9);
		    	 	}
		    	 	$("#Pressure9").val(Pre9);*/
		    	 	
		    	 	if(Tem10==-40){
		    	 		$("#Temperature10").val(0);
		    	 	}else{
		    	 		$("#Temperature10").val(Tem10);
		    	 	}
		    	 	if(Hum10==5){
		    	 		$("#Humidity10").val(0);
		    	 	}else{
		    	 		$("#Humidity10").val(Hum10);
		    	 	}
		    	 	$("#Pressure10").val(Pre10);
		    	 	
		    	 	if(Tem12==-40){
		    	 		$("#Temperature12").val(0);
		    	 	}else{
		    	 		$("#Temperature12").val(Tem12);
		    	 	}
		    	 	if(Hum12==5){
		    	 		$("#Humidity12").val(0);
		    	 	}else{
		    	 		$("#Humidity12").val(Hum12);
		    	 	}
		    	 	$("#Pressure12").val(Pre12);
		    	 	
		    	 	if(Tem13==-40){
		    	 		$("#thtem").html(0);
		    	 	}else{
		    	 		$("#thtem").html(Tem13);
		    	 	}
		    	 	if(Hum13==5){
		    	 		$("#thhum").html(0);
		    	 	}else{
		    	 		$("#thhum").html(Hum13);
		    	 	}
		    	 		var timestamp = Date.parse(new Date());
						Date.prototype.format = function(format){
				    		 var o = {
				    		 "M+" : this.getMonth()+1, 
				    		 "d+" : this.getDate(),    
				    		 "h+" : this.getHours(),  
				    		 "m+" : this.getMinutes(), 
				    		 "s+" : this.getSeconds(), 
				    		 "q+" : Math.floor((this.getMonth()+3)/3),  
				    		 "S" : this.getMilliseconds() 
				    		 };
				    		 if(/(y+)/.test(format)) format=format.replace(RegExp.$1,
				    		 (this.getFullYear()+"").substr(4 - RegExp.$1.length));
				    		 for(var k in o)if(new RegExp("("+ k +")").test(format))
				    		 format = format.replace(RegExp.$1,
				    		 RegExp.$1.length==1 ? o[k] :
				    		 ("00"+ o[k]).substr((""+ o[k]).length));
				    		 return format;
				    		};
				    		
				    		var oq =new Date(aa[1].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svq=Date.parse(oq.replace(/-/g,"/"));
				    		if(timestamp-svq>60000){
				    			$("#run").val('#');
					    	 	/*$("#testre").val('#');
					    	 	$("#testre2").val('#');*/
					    	 	$("#discharge").val('#');
					    	 	$("#Temper").val('#');
					    	 	$("#current_u").val('#');
					    	 	/*$("#current_v").val('#');
					    	 	$("#current_w").val('#');*/
				    		}
				    		
				    		var ow =new Date(ab[1].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svw=Date.parse(ow.replace(/-/g,"/"));
				    		if(timestamp-svw>60000){
				    			$("#run2").val('#');
					    	 	/*$("#testre3").val('#');
					    	 	$("#testre4").val('#');*/
					    	 	$("#discharge2").val('#');
					    	 	$("#Temper2").val('#');
					    	 	$("#current_u2").val('#');
					    	 	/*$("#current_v2").val('#');
					    	 	$("#current_w2").val('#');*/
				    		}
				    		var oe =new Date(ac[1].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var sve=Date.parse(oe.replace(/-/g,"/"));
				    		if(timestamp-sve>60000){
				    			$("#run3").val('#');
					    	 	/*$("#testre5").val('#');
					    	 	$("#testre6").val('#');*/
					    	 	$("#discharge3").val('#');
					    	 	$("#Temper3").val('#');
					    	 	$("#current_u3").val('#');
					    	 	/*$("#current_v3").val('#');
					    	 	$("#current_w3").val('#');*/
				    		}
				    		var or =new Date(ad[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svr=Date.parse(or.replace(/-/g,"/"));
				    		if(timestamp-svr>60000){
				    			$("#Pressure3").val('#');
					    	 	$("#Temperature3").val('#');
					    	 	$("#Humidity3").val('#');
				    		}
				    		var ot =new Date(ae[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svt=Date.parse(ot.replace(/-/g,"/"));
				    		if(timestamp-svt>60000){
				    			$("#Pressure").val('#');
					    	 	$("#Temperature").val('#');
					    	 	$("#Humidity").val('#');
				    		}
				    		var oo =new Date(af[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svl=Date.parse(oo.replace(/-/g,"/"));
				    		if(timestamp-svl>60000){
				    			$("#Pressure2").val('#');
					    	 	$("#Temperature2").val('#');
					    	 	$("#Humidity2").val('#');
				    		}
				    		var oy =new Date(ag[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svy=Date.parse(oy.replace(/-/g,"/"));
				    		if(timestamp-svy>60000){
				    			$("#Pressure4").val('#');
					    	 	$("#Temperature4").val('#');
					    	 	$("#Humidity4").val('#');
				    		}
				    		var ou =new Date(ah[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svu=Date.parse(ou.replace(/-/g,"/"));
				    		if(timestamp-svu>60000){
				    			$("#Pressure5").val('#');
					    	 	$("#Temperature5").val('#');
					    	 	$("#Humidity5").val('#');
				    		}
				    		/*var oi =new Date(ai[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svi=Date.parse(oi.replace(/-/g,"/"));
				    		if(timestamp-svi>60000){
				    			$("#Pressure6").val('#');
					    	 	$("#Temperature6").val('#');
					    	 	$("#Humidity6").val('#');
				    		}
				    		var op =new Date(aj[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svp=Date.parse(op.replace(/-/g,"/"));
				    		if(timestamp-svp>60000){
				    			$("#Pressure7").val('#');
					    	 	$("#Temperature7").val('#');
					    	 	$("#Humidity7").val('#');
				    		}
				    		var oa =new Date(ak[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var sva=Date.parse(oa.replace(/-/g,"/"));
				    		if(timestamp-sva>60000){
				    			$("#Pressure8").val('#');
					    	 	$("#Temperature8").val('#');
					    	 	$("#Humidity8").val('#');
				    		}
				    		var os =new Date(al[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svs=Date.parse(os.replace(/-/g,"/"));
				    		if(timestamp-svs>60000){
				    			$("#Pressure9").val('#');
					    	 	$("#Temperature9").val('#');
					    	 	$("#Humidity9").val('#');
				    		}*/
				    		var od =new Date(az[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svd=Date.parse(od.replace(/-/g,"/"));
				    		if(timestamp-svd>60000){
				    			$("#Pressure10").val('#');
					    	 	$("#Temperature10").val('#');
					    	 	$("#Humidity10").val('#');
				    		}
				    		var of =new Date(ax[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svf=Date.parse(of.replace(/-/g,"/"));
				    		if(timestamp-svf>60000){
				    			$("#Pressure12").val('#');
					    	 	$("#Temperature12").val('#');
					    	 	$("#Humidity12").val('#');
				    		}
				    		var og =new Date(am[0].recordingTime.time).format("yyyy-MM-dd hh:mm:ss");
				    		var svg=Date.parse(og.replace(/-/g,"/"));
				    		if(timestamp-svg>60000){
				    			$("#thtem").html('#');
					    	 	$("#thhum").html('#');
				    		}
				   		 /*var url2='/JianHeSysTest/new/havepolice.do';
				   		 $.ajax({
				   			 async : true,
				   			 type : 'post',
				   		     data : {array:JSON.stringify(array),array2:JSON.stringify(array2),arrayt:JSON.stringify(arrayt),na:na,na2:na2,acpl:acpl,acpl2:acpl2,acpl3:acpl3,dry:dry,dry2:dry2,hd3:hd3,hd4:hd4,rm:rm},
				   		     dataType : 'html',
				   		     cache : false,
				   		     url : url2,
				   		     timeout:1000,
				   		     success:function(result){
				   		    	 if(result=="success"){
				   		    		//alert(array)	
				   		    	 }
				   		     },
				   		     error:function(){
				   		    		//alert("请求错误！");
				   		    	},
				   		 });*/	
		     	},
		      error:function(){
		    		//alert("请求错误！");
		    	},
		 });
			
	};
	
	setInterval(function() { 
    	myrefresh();}, 3000);
	
	
	/*function myrefresh2(){
		var url='/JianHeSysTest/new/upStatus.html?time='+new Date();
		 $.ajax({
			 async : true,
			 type : 'post',
		     dataType : 'html',
		     cache : false,
		     url : url,
		     timeout:1000,
		     success:function(result){
		    	 if(result=='status'){
		    	 }
		 },
		 error:function(){
	    		//alert("请求错误！");
	    },
	});
	}
	setInterval(function() { 
    	myrefresh2();}, 3000);*/