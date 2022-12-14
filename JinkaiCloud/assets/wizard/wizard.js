(function (b, c) {
    var a = function (f, e) {
        var d;
        this.$element = b(f);
        this.options = b.extend({}, b.fn.wizard.defaults, e);
        this.currentStep = 1;
        this.numSteps = this.$element.find("li").length;
        /*Customized to Enable Out Of Wizard Buttons*/
        this.$prevBtn = $(this.$element.attr("data-action")).find("a.btn-prev").attr("disabled", "disabled");
        this.$nextBtn = $(this.$element.attr("data-action")).find("a.btn-next");
        this.$finishBtn = $(this.$element.attr("data-action")).find("a.btn-finish").attr("disabled", "disabled");
        /*End Customized to Enable Out Of Wizard Buttons*/
        //d = this.$nextBtn.children().detach();
        //this.nextText = b.trim(this.$nextBtn.text());
        //this.$nextBtn.append(d);
        this.$prevBtn.on("click", b.proxy(this.previous, this));
        this.$nextBtn.on("click", b.proxy(this.next, this));
        this.$element.on("click", "li.complete", b.proxy(this.stepclicked, this));
        this.$stepContainer = this.$element.data("target") || "body";
        this.$stepContainer = b(this.$stepContainer)
    };
    a.prototype = {
        constructor: a,
        setState: function () {
            var n = (this.currentStep > 1);
            var o = (this.currentStep === 1);
            var d = (this.currentStep === this.numSteps);
            var licount = $('#' + this.$element[0].id).find("ul li").length;

            this.$prevBtn.attr("disabled", (o === true || n === false));
            this.$nextBtn.attr("disabled", this.currentStep == licount ? true : false);
            this.$finishBtn.attr("disabled", this.currentStep == licount ? false : true);
            if ((this.currentStep == licount ? false : true) == false) {
                var $event = this.$element;
                this.$finishBtn.on("click", function () {
                    $event.trigger("finish");
                });
            } else {
                this.$finishBtn.unbind("click");
            }
            //next
            var h = this.$nextBtn.data();
            if (h && h.last) {
                this.lastText = h.last;
                if (typeof this.lastText !== "undefined") {
                    var l = (d !== true) ? this.nextText : this.lastText;
                    var f = this.$nextBtn.children().detach();
                    this.$nextBtn.text(l).append(f)
                }
            }
            var j = this.$element.find("li");
            j.removeClass("actives").removeClass("complete");
            var m = "li:lt(" + (this.currentStep - 1) + ")";
            var g = this.$element.find(m);
            g.addClass("complete");
            var e = "li:eq(" + (this.currentStep - 1) + ")";
            var k = this.$element.find(e);
            k.addClass("actives");
            var i = k.data().target;
            this.$stepContainer.find(".step-pane").removeClass("actives");
            b(i).addClass("actives");
            this.$element.trigger("changed")
        },
        stepclicked: function (h) {
            var d = b(h.currentTarget);
            var g = this.$element.find("li").index(d);
            var f = b.Event("change");
            this.$element.trigger(f, {
                step: g + 1,
                direction: "stepclicked",
                currentStep: g + 1
            });
            if (f.isDefaultPrevented()) {
                return
            }
            this.currentStep = (g + 1);
            this.setState()
        },
        previous: function () {
            var d = (this.currentStep > 1);
            if (d) {
                var f = b.Event("change");
                this.$element.trigger(f, {
                    step: this.currentStep - 2,
                    direction: "previous",
                    currentStep: this.currentStep - 1
                });
                if (f.isDefaultPrevented()) {
                    return
                }
                this.currentStep -= 1;
                this.setState()
            }
        },
        next: function () {
            var g = (this.currentStep + 1 <= this.numSteps);
            var d = (this.currentStep === this.numSteps);
            if (g) {
                var f = b.Event("change");
                this.$element.trigger(f, {
                    step: this.currentStep,
                    direction: "next",
                    currentStep: this.currentStep + 1
                });
                if (f.isDefaultPrevented()) {
                    return
                }
                this.currentStep += 1;
                this.setState()
            } else {
                if (d) {
                    this.$element.trigger("finished")
                }
            }
        },
        selectedItem: function (d) {
            return {
                step: this.currentStep
            }
        }
    };
    b.fn.wizard = function (e, g) {
        var f;
        var d = this.each(function () {
            var j = b(this);
            var i = j.data("wizard");
            var h = typeof e === "object" && e;
            if (!i) {
                j.data("wizard", (i = new a(this, h)))
            }
            if (typeof e === "string") {
                f = i[e](g)
            }
        });
        return (f === c) ? d : f
    };
    b.fn.wizard.defaults = {};
    b.fn.wizard.Constructor = a;
    b(function () {
        jinkai.loadStyle("../../assets/wizard/wizard.css");
        b("body").on("mousedown.wizard.data-api", ".wizard", function () {
            var d = b(this);
            if (d.data("wizard")) {
                return
            }
            d.wizard(d.data())
        })
    })
})(window.jQuery);