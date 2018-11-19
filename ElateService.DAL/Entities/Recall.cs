using System;

namespace ElateService.DAL.Entities
{
    public class Recall
    {
        public int RecallId { get; set; }
        public string CustomerCommentForExecutor { get; set; }
        public int? CustomerMarkForExecutor { get; set; }
        public string ExecutorCommentForCustomer { get; set; }
        public int? ExecutorMarkForCustomer { get; set; }
        public Indent Indent { get; set; }


        public bool Equals(Recall other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return RecallId.Equals(other.RecallId);
        }


        public override int GetHashCode()
        {
            int hashRecallId = RecallId.GetHashCode();            

            return hashRecallId;
        }
    }
}
