wordutopiaApp.service("wordutopiaService", function ($http) {

    var baseUrl = $("base").first().attr("href");

    this.getWord = function (previousWordIds, type, currentWordId ) {
        var response = $http({
            method: "post",
            url: baseUrl + "Wordutopia/GetWordForHome",
            params: {
                previousWordIds: previousWordIds,
                type: type,
                currentWordId: currentWordId
            }
        });

        return response;
    }

    this.addToFavourite = function (wordId) {
        var response = $http({
            method: "post",
            url: baseUrl + "Wordutopia/AddToFavourite",
            params: {
                id: wordId
            }
        });
        return response;
    }

    this.addToSkippedList = function (wordId) {
        var response = $http({
            method: "post",
            url: baseUrl + "Wordutopia/AddToSkippedList",
            params: {
                id: wordId
            }
        });
        return response;
    }

    this.getWordById = function (wordId) {
        var response = $http({
            method: "get",
            url: baseUrl + "Wordutopia/GetWordById",
            params: {
                id: wordId
            }
        });
        return response;
    }
    this.deleteFromReviewWords = function (wordId) {
        var response = $http({
            method: "get",
            url: baseUrl + "Wordutopia/DeleteFromReviewWords",
            params: {
                wordId: wordId
            }
        });
        return response;
    }
    
});