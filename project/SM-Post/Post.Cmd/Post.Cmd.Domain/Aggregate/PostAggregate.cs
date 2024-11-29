using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregate
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string _author;

        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool Active
        {
            get => _active; set => _active = value;
        }

        public PostAggregate()
        {

        }

        #region PostCreatedEvent
        //command that creates an instance of aggregate should always be handled in constructor of the aggregate
        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DateCreated = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {// used to alter state of event
            _id = @event.Id;
            _author = @event.Author;
            _active = true;
        }

        #endregion

        #region MessageUpdatedEvent
        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot edit the message of inactive post");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"The value of  {nameof(message)} cannot be null or empty. Please provide a valid {nameof(message)}!");
            }

            RaiseEvent(new MessageUpdatedEvent
            {
                Id = _id,
                Message = message
            });
        }

        public void Apply(MessageUpdatedEvent @event)
        {// used to alter state of event
            _id = @event.Id;
        }

        #endregion

        #region  PostLikedEvent
        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot like the inactive post");
            }
            RaiseEvent(new PostLikedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostLikedEvent @event)
        {// used to alter state of event
            _id = @event.Id;
        }
        #endregion

        #region CommentAddedEvent
        public void AddComment(string comment, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot add comment to an inactive post");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"The value of  {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!");
            }

            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                UserName = username,
                CommentDate = DateTime.Now
            });
        }

        public void Apply(CommentAddedEvent @event)
        {// used to alter state of event
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.UserName));
        }

        #endregion

        #region CommentUpdatedEvent
        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot add comment to an inactive post");
            }

            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"You are nit allowed to edit the comment made by another user!");
            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                UserName = username,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {// used to alter state of event
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.UserName);
        }
        
        #endregion

        #region CommentRemovedEvent
        public void RemoveComment(Guid commentId, string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot add comment to an inactive post");
            }

            if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException($"You are nit allowed to edit the comment made by another user!");
            }

            RaiseEvent(new CommentRemovedEvent
            {
                Id = _id,
                CommentId = commentId,
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {// used to alter state of event
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }
        
        #endregion

        #region PostRemovedEvent
        public void DeletePost(string username)
        {
            if (!_active)
            {
                throw new InvalidOperationException("The post has already been deleted");
            }

            if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to delete post made by someone else!");
            }

            RaiseEvent(new PostRemovedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostRemovedEvent @event)
        {// used to alter state of event
            _id = @event.Id;
            _active = false;
        }        
        #endregion

    }
}