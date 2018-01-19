
const $body = $("body");
$body.on("click", ".date", function () {
    console.log("**yang datepicker begin.");
    let $dpicker = $body.find(".datepicker");
    if ($dpicker.length == 0) {
        $body.append(`
<div class='datepicker'>
<div class='mask mask-top' onclick='$(this).parent().hide();'></div>
<div class='datepicker-box'>
 <div class='title center'>日期选择</div>
    <div class='weekday center'>星期二</div>
    <div class='datepicker-content'>
        <div class=item>
            <i class='fa fa-sort-asc'></i>
            <select id='year'> </select>
        </div>
        <div class=item>
            <select id='month'> </select>
        </div>
        <div class=item>
            <select id='day'> </select>
        </div>
         
    </div>
</div>
</div>
`);
        $dpicker = $body.find(".datepicker");
        const $year = $dpicker.find("#year");
        let year = new Date().getFullYear();
         
        for (var i = year - 90; i < year + 10; i++) {
            $year.append("<option value=" + i +(i==year?" selected":"") + ">" + i + "年</option>");
        }
        
    } else {
        $dpicker.show();
    }



})
