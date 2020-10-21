// 导出页面为PDF格式
import html2Canvas from "html2canvas";
import JsPDF from "jspdf";
export default {
  install(Vue, options) {
    Vue.prototype.getPdf = function (title,id,conf) {
      var element = document.querySelector(id);//("#pdfDom"); // 这个dom元素是要导出pdf的div容器
      // setTimeout(() => {
      html2Canvas(element).then(function (canvas) {
        var contentWidth = canvas.width;
        var contentHeight = canvas.height;

        //一页pdf显示html页面生成的canvas高度;
        var pageHeight = (contentWidth / 592.28) * 841.89;
        //未生成pdf的html页面高度
        var leftHeight = contentHeight;
        //页面偏移
        var position = 0;
        //a4纸的尺寸[595.28,841.89]，html页面生成的canvas在pdf中图片的宽高
        // var imgWidth = 841.89;
        // var imgHeight = (841.89 / contentWidth) * contentHeight;
        var imgWidth = 595.28;
        var imgHeight = (592.28 / contentWidth) * contentHeight;
        // setTimeout(() => {
          var pageData = canvas.toDataURL("image/svg", 1.0);

          //console.log(pageData);
          var pdf = new JsPDF(conf.direction, conf.unit, conf.size);
          //有两个高度需要区分，一个是html页面的实际高度，和生成pdf的页面高度(841.89)
          //当内容未超过pdf一页显示的范围，无需分页
          if (leftHeight < pageHeight) {
            pdf.addImage(pageData, "svg", 10, 10, imgWidth-20, imgHeight-20);
          } else {
            while (leftHeight > 0) {
              pdf.addImage(pageData, "svg", 10, position, imgWidth, imgHeight);
              leftHeight -= pageHeight;
              // position -= 595.28;
              position -= 841.89;
              //避免添加空白页
              if (leftHeight > 0) {
                pdf.addPage();
              }
            }
          }
          //console.log('生成完毕');

          pdf.save(title + ".pdf");
        // }, 3 * 1000);
      });
      // }, 10 * 1000);
    };
  }
};
