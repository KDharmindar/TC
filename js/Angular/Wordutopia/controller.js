wordutopiaApp.controller("wordutopiaController", function ($scope,$window, wordutopiaService) {
    "use strict";
    var vm = this;
    this.initializeController = function (wordutopiaData) {
        vm.wordutopia = wordutopiaData;
        vm.isShowAllClicked = false;
    };

    this.getWord = function ($event) {
        var elementId = angular.element($event.target).attr("id");
        var type = "";
        switch (elementId) {
            case "lnkPrevious":
                type = "Previous";
                break;
            case "lnkNext":
                type = "Next";
                break;
        }
        if (type == "Previous" && vm.wordutopia.ListOfPreviousWords.length <= 0) {
            return false;
        }


        var promiseWord = wordutopiaService.getWord(vm.wordutopia.ListOfPreviousWords, type, vm.wordutopia.WordID);
        promiseWord.then(function (resp) {
            vm.wordutopia = resp.data;

        }, function (err) {
            console.log(err);
            alert('error');

        })
    }

    this.addToFavourite = function (wordId) {
        var response = wordutopiaService.addToFavourite(wordId);
        response.then(function (resp) {
            alert(resp.data);
        }, function (error) {
            alert(error.data);
        })
    }

    this.addToSkippedList = function (wordId) {
        var response = wordutopiaService.addToSkippedList(wordId);
        response.then(function (resp) {
            alert(resp.data);
        }, function (error) {
            alert(error.data);
        })
    }

    this.SetPanelValue = function (wordId,type) {
        var result = getFilteredWordutopia(wordId);
        clearPanel();
        if (vm.isShowAllClicked === false)
        {
            alert('Please try to complete the list of words that appeared in your latest session before using the usual feature buttons. When done, or you get stuck, click the SHOW ALL button and the full listing will appear');
            return false;
        }
        setPanelValues(result, type);
    }

    this.DeleteFromReviewWords = function (wordId) {
        wordutopiaService.deleteFromReviewWords(wordId);
    }

    function setPanelValues(result,type) {
        switch (type) {
            case "hint":
                $("#pHint").text(result.Hint);
                $("#accordion").accordion({
                    active: 1
                });
                break;
            case "phrase":
                $("#pPhrase").text(result.Phrase);
                $("#accordion").accordion({
                    active: 0
                });
                break;

            case "definition":
                $("#pDefinition").text(result.Definition);
                $("#accordion").accordion({
                    active: 2
                });
                break;

            case "synonym":
                $window.open('http://www.thesaurus.com/browse/' + result.Word, '_blank');

        }

    }

    function getFilteredWordutopia(wordId) {
        return vm.wordutopia.filter(function (obj) {
            return obj.WordID == wordId;
        })[0];
    }

    function clearPanel() {
        $("#pHint").text('');
        $("#pPhrase").text('');
        $("#pDefinition").text('');
        $("#accordion").accordion({
            active: 0
        });
    }

    this.ShowAllWords = function () {
        $.each(vm.wordutopia,
            function (i, v) {
                $("#sp-" + v.WordID).text(v.Word);
            }
        );
        vm.isShowAllClicked = true;
    }
})