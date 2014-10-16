(function ($) {
    var $fileId = $('#fileId');
    var $tagsElement = $('#fileTags');
    var $descriptionElement = $('#fileDescription');
    var $commentsPanel = $('#commentsPanel');

    var fileId = $fileId.text();
    var tagsValue = $tagsElement.val();
    var descriptionValue = $descriptionElement.val();

    $('#cancel-button').on('click', function () {
        $tagsElement.val(tagsValue);
        $descriptionElement.val(descriptionValue);
    });

    $('#remove-button').on('click', function () {
        $('#removeForm').submit();
    });

    var getParameterByName = function(url, name) {
        var match = new RegExp('[?&]' + name + '=([^&]*)').exec(url);
        return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
    };

    var loadComments = function(page, messageType) {
        $commentsPanel.load(
            '/Files/GetCommentsForFile',
            {
                fileId: fileId,
                page: page,
                messageType: messageType
            },
            function() {                
                var $pagination = $('.pagination');
                var $newCommentForm = $('#newCommentForm');                    

                $commentsPanel.on('change', 'input.activateCommentCheckbox', function() {
                    var $checkbox = $(this);
                    var commentId = $checkbox.val();
                    var isActive = $checkbox.prop('checked');
                    $.post(
                        '/Files/ChangeCommentState',
                        {
                            fileId: fileId,
                            commentId: commentId,
                            isActive: isActive
                        },
                        function(response) {
                            if (response) {
                                var $div = $checkbox.parent().parent().parent().parent();
                                if (isActive) {
                                    $div.removeClass('panel-danger');
                                    $div.addClass('panel-info');
                                } else {
                                    $div.removeClass('panel-info');
                                    $div.addClass('panel-danger');
                                }                                    
                            }
                        }
                    );
                });

                $commentsPanel.on('submit', 'form.deleteCommentForm', function(event) {
                    event.preventDefault();
                    var $form = $(this);
                    $.post(
                        $form.attr('action'),
                        $form.serialize(),
                        function(response) {                                
                            loadComments(null, response);
                        }
                    );
                });

                $newCommentForm.on('submit', function(event) {
                    event.preventDefault();
                    var $form = $(this);
                    $.post(
                        $form.attr('action'),
                        $form.serialize(),
                        function(response) {
                            loadComments(null, response);
                        }
                    );
                });

                $pagination.on('click', 'a', function(event) {
                    event.preventDefault();
                    var href = $(this).attr('href');
                    var pageNumber = getParameterByName(href, 'page');
                    loadComments(pageNumber, null);
                });
            }
        );
    };

    loadComments(null, null);
})(jQuery);