//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace telstarapp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Package
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Package()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int PackageId { get; set; }
        public double Weight { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public int Width { get; set; }
        public Nullable<byte> Recommeded { get; set; }
        public Nullable<int> SpecialGoods { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        public virtual SpecialGood SpecialGood { get; set; }
    }
}
