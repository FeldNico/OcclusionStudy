let qWindow = {};

document.addEventListener("DOMContentLoaded", function(event) {
    window.addEventListener("qAnswered", (event) => {
        console.log("answered");
        qWindow.close();
    }, false);
});

mergeInto(LibraryManager.library, {
  StartQuestionnaire: function (type,codename) {
      console.log("https://www.unipark.de/uc/occlusion/?a="+type+"&b="+codename)
      document.domain = "unipark.de";
      qWindow = window.open("https://www.unipark.de/uc/occlusion/?a="+type+"&b="+codename);
  }
});