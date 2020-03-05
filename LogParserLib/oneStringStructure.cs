using System;

namespace LogParser
{
    public sealed class oneStringStructure
    {
        public oneStringStructure (
            DateTime Point_TimeStamp,
            
            UInt16 Iteration_Number,

            UInt16 Device_SerNum,
            string Device_MBAddress, // полный адрес aka 40001 

            string Cell_Alias,
            double Cell_Value, // СКО. Для булевых перевод обратно в дискретное значение [false, true]
            int ChannelNum
        )
        {
            this.Point_TimeStamp = Point_TimeStamp;

            this.Iteration = Iteration_Number;

            this.Device_SerNum = Device_SerNum;
            this.Device_MBAddress = Device_MBAddress;
            
            this.Cell_Alias = Cell_Alias;
            this.Cell_Value = Cell_Value;
            this.ChannelNum = ChannelNum;
        }

        public DateTime Point_TimeStamp { get; private set; }
        public UInt16 Iteration { get; private set; }
        public UInt16 Device_SerNum { get; private set; }
        public string Device_MBAddress { get; private set; }
        [Obsolete("Использовать ChannelNum по возможности", false)]
        public string Cell_Alias { get; private set; }
        public double Cell_Value { get; private set; }
        public int ChannelNum { get; private set; }

        public int CompareTo(object obj)
        {
            return CompareTo(obj, StringComparison.CurrentCulture);
        }

        public int CompareTo(object obj, StringComparison stringComparison)
        {
            var member = obj as oneStringStructure;
            // Переменная member будет равна null в двух случаях:
            // 1) если null-у была равна переменная obj;
            // 2) если obj имеет тип, несовместимый (не являещийся наследником)
            //    с DropDownMember.
            // Таким образом, мы защищаемся и от того, что нам может быть передано
            // значение null, и от того, что нам может быть передана ссылка на
            // объект неверного типа.
            if (!object.ReferenceEquals(member, null))
            {
                if (
                (this.Point_TimeStamp == member.Point_TimeStamp) &&
                (this.Iteration == member.Iteration) &&
                (this.Device_SerNum == member.Device_SerNum) &&
                (this.Device_MBAddress == member.Device_MBAddress) &&
                (this.Cell_Alias == member.Cell_Alias) &&
                (this.Cell_Value == member.Cell_Value)
                == true)
                {
                    return 0;
                }
            }
            return -1;
        }

        public override bool Equals(Object obj)
        {
            return CompareTo(obj, StringComparison.CurrentCulture) == 0;
        }

        public override int GetHashCode()
        {
            return
                this.Point_TimeStamp.GetHashCode() ^
                this.Iteration.GetHashCode() ^
                this.Device_SerNum.GetHashCode() ^
                this.Device_MBAddress.GetHashCode() ^
                this.Cell_Alias.GetHashCode() ^
                this.Cell_Value.GetHashCode();
        }

        public static bool operator ==(oneStringStructure m1, oneStringStructure m2)
        {
            // Если оба объекта будут равны null, то это условие сработает, и
            // и дальнейших проверок производиться не будет. Таким образом,
            // с одной стороны мы защищаемся от передачи null-значений, а с другой -
            // правильно реагируем на передачу сразу двух null-значений.
            // Как положительный эффект, мы так же достигаем некоторого ускорения 
            // выполнения кода при сравнении ссылок на один и тот же объект
            // (ведь ReferenceEquals работает очень быстро).
            // Обратите особое внимение на то, что сравнивать ссылки вот так:
            // «m1 == m2» ни в коем случае нельзя! Такой код приведет к рекурсивному
            // вызову этого же самого оператора и, в итоге, к переполнению стека.
            // Если уж вы очень хотите избавиться от использования ReferenceEquals,
            // то позаботьтесь о том, чтобы привести один из операндов к object:
            // «(object)m1 == m2». Это приведет (в C#) к сравнению ссылок, так как 
            // класс object не реализует операторов сравнения.
            if (object.ReferenceEquals(m1, m2))
                return true;

            // В этом месте, если m1 равна null, то m1 уже не может быть 
            // равна null. Стало быть, объекты не равны, и можно вернуть false.
            if (object.ReferenceEquals(m1, null))
                return false;

            // В этом месте null не может быть ни в одном параметре, так что
            // можно смело вызывать Equals.
            // Equals является виртуальным, так что пользователи нашего класса
            // смогут переопределить его, если им потребуется изменить поведение
            // операции сравнения.
            return m1.Equals(m2);
        }

        public static bool operator !=(oneStringStructure m1, oneStringStructure m2)
        {
            // Здесь управление передается оператору «==», реализованному выше.
            // Так как он защищен от проблем с null, то можно делать это без
            // предварительных проверок.
            return !(m1 == m2);
        }
    }
}
