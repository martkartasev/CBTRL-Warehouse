import os
from dataclasses import dataclass

import tyro

from cleanrl_ppo import Args as BaseArgs
from cleanrl_ppo import run_ppo
from unity_env_factory import make_unity_env_factory


@dataclass
class Args(BaseArgs):
    exp_name: str = os.path.basename(__file__)[: -len(".py")]
    env_id: str = "Place"
    total_timesteps: int = 60000000
    hidden_size: int = 128


make_env = make_unity_env_factory(
    base_port=50010,
    scene_load="LearnPlace_Discrete",
    log_file_prefix="place_",  # Set to "" to disable logging files
)


if __name__ == "__main__":
    run_ppo(tyro.cli(Args), make_env)
