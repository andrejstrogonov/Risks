using DataProcessLibrary.Analys;
using DataProcessLibrary.CommonAnalys;
using DataProcessLibrary.Flexibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Riscks.ViewModel
{
    public class ProgressLineViewModel:ViewModel
    {
        int maximum;
        public int Maximum { get
            {
                return maximum;
            }
            set
            {
                maximum = value;
                OnPropertyChanged(nameof(Maximum));
            }
        }

        int currentProgress;
        public int CurrentProgress
        {
            get
            {
                return currentProgress;
            }
            set
            {
                
                currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
                OnPropertyChanged(nameof(AmountProgress));
                OnPropertyChanged(nameof(TimeProgress));
                OnPropertyChanged(nameof(Procent));
            }
        }

        public DateTime StartTime { get; set; }

        public string AmountProgress
        {
            get
            {
                return currentProgress.ToString()+" / "+Maximum.ToString();
            }
        }

        public string TimeProgress
        {
            get
            {
                var time = DateTime.Now - StartTime;
                var ticks = time.TotalSeconds;
                if(ticks!=0 && currentProgress != 0)
                {
                    int least = Maximum - currentProgress;
                    var leastTime =  least * (int)ticks / currentProgress;
                    string answer = "";
                    if (leastTime == 0)
                        return "Почти готово";
                        var sec = (int)leastTime % 60;
                        leastTime /= 60;
                        answer = sec.ToString() + " сек ";
                    if (leastTime > 0)
                    {
                        var min = (int)leastTime % 60;
                        leastTime /= 60;
                        answer = min.ToString() + " мин ";
                        if (leastTime > 0)
                        {
                            var hour = (int)leastTime % 60;
                            leastTime /= 60;
                            answer = hour.ToString() + " ч ";
                            if (leastTime > 0)
                            {
                                var days = (int)leastTime % 24;
                                leastTime /= 24;
                                answer = days.ToString() + " д ";
                                if (leastTime > 0)
                                {
                                    return "Время оценивается...";
                                }
                            }
                        }
                    }

                    return "Осталось: " + answer;

                }
                return "Время оценивается...";
            }
        }

        public string Procent { get
            {
                return Math.Round(((double)currentProgress / (double)maximum) * 100).ToString()+"%";
            } 
        }

        string stateProgress;
        public string StateProgress 
        { 
            get
            {
                return stateProgress;
            }
            set 
            {
                stateProgress = value;
                OnPropertyChanged(nameof(StateProgress));
            } 
        }

        private bool isAnalys;
        public ProgressLineViewModel(string Title, bool analys=true)
        {
            isAnalys = analys;
            StopIsVisible = Visibility.Visible;
            
            this.Title = Title;
            StartTime = DateTime.Now;
            Maximum = 100;
            SimplexStage.NeedToStop = false;
            SingleDecidionStage.StopFromInterface = false;
        }          
        
        public string Title { get; set; }
        public void SetState(string state)
        {
            CurrentProgress = 0;
            StateProgress = state;
        }

        

        public void SetMaximum(int max)
        {
            Maximum = max;
            
        }
        public void IncreaseProgress()
        {
            CurrentProgress += 1;
        }

        public void StopProgress()
        {
            CurrentProgress = Maximum;
        }

        public Visibility StopIsVisible { get; set; }

        public void StopProgressFromInterface()
        {
            if (isAnalys)
            {
                SimplexStage.NeedToStop = true;
                SingleDecidionStage.StopFromInterface = true;
            }
            else
            {
                FlexibilityAnalys.StopFromInterface = true;
            }
        }
    }
}
